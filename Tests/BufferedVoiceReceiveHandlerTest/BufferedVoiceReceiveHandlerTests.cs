using System.Buffers.Binary;

using NetCord.Gateway.Voice;

namespace BufferedVoiceReceiveHandlerTest;

[TestClass]
public sealed class BufferedVoiceReceiveHandlerTests(TestContext context)
{
    private const uint DefaultSsrc = 1234;
    private const uint DefaultFrameSamples = 960;

    /// <summary>
    /// Captures every field emitted by the handler for thorough verification.
    /// For delivered packets: Tag contains the payload marker; SamplesPerChannel, DecodeFec, FecTag are all zero.
    /// For lost packets: Tag is zero; SamplesPerChannel/DecodeFec/FecTag contain loss metadata.
    /// </summary>
    private record struct ReceivedPacket(
        uint Ssrc,
        ushort SequenceNumber,
        uint Timestamp,
        bool IsLost,
        int Tag,
        int SamplesPerChannel,
        bool DecodeFec,
        int FecTag);

    private static (BufferedVoiceReceiveHandler Handler, List<ReceivedPacket> Packets) CreateHandler(
        BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        BufferedVoiceReceiveHandler handler = new(configuration);
        List<ReceivedPacket> packets = [];

        // Lock protects the shared list when timer callbacks fire concurrently
        // for different SSRCs on separate ThreadPool threads.
        object lockObj = new();

        handler.VoiceReceive += data =>
        {
            bool isLost = data.IsLost;
            int tag = 0;
            int samplesPerChannel = 0;
            bool decodeFec = false;
            int fecTag = 0;

            if (isLost)
            {
                var lost = data.AsLost();
                samplesPerChannel = lost.SamplesPerChannel;
                decodeFec = lost.DecodeFec;
                if (decodeFec)
                    fecTag = BinaryPrimitives.ReadInt32LittleEndian(lost.FecData[2..]);
            }
            else
            {
                tag = BinaryPrimitives.ReadInt32LittleEndian(data.Frame[2..]);
            }

            lock (lockObj)
                packets.Add(new(data.Ssrc, data.SequenceNumber, data.Timestamp, isLost, tag, samplesPerChannel, decodeFec, fecTag));
        };

        return (handler, packets);
    }

    private static VoiceReceiveContext Packet(
        ushort seq, uint timestamp, uint ssrc = DefaultSsrc,
        int tag = 0, int frameSamples = (int)DefaultFrameSamples)
    {
        var toc = BuildOpusToc(frameSamples);
        if (toc.Length is 1)
            toc = [toc[0], 0]; // Pad to 2 bytes so tag always starts at offset 2

        var tagBytes = (stackalloc byte[4]);
        BinaryPrimitives.WriteInt32LittleEndian(tagBytes, tag);

        byte[] frame = [.. toc, .. tagBytes];
        return new(new(ssrc, timestamp, 0x78, seq, tag), frame);
    }

    /// <summary>
    /// Generates valid Opus TOC bytes encoding the given total samples-per-channel.
    /// Supports standard Opus frame sizes and any multiple of 120 up to 63 frames.
    /// </summary>
    private static byte[] BuildOpusToc(int targetSamples, int fs = 48000)
    {
        var configs = new (int Samples, byte BaseToc)[]
        {
            (fs * 60 / 1000, 0x18),  // 2880 samples - SILK 60ms
            ((fs << 2) / 100, 0x10), // 1920 samples - SILK 40ms
            ((fs << 1) / 100, 0x08), // 960  samples - SILK 20ms
            ((fs << 0) / 100, 0x00), // 480  samples - SILK 10ms
            ((fs << 1) / 400, 0x88), // 240  samples - CELT 5ms
            ((fs << 0) / 400, 0x80), // 120  samples - CELT 2.5ms
        };

        foreach (var (samples, baseToc) in configs)
        {
            if (targetSamples % samples != 0)
                continue;

            int frames = targetSamples / samples;
            if (frames > 63)
                continue;

            return frames switch
            {
                1 => [(byte)(baseToc | 0x00)],
                2 => [(byte)(baseToc | 0x01)],
                _ => [(byte)(baseToc | 0x03), (byte)frames],
            };
        }

        throw new ArgumentException(
            $"Cannot generate Opus TOC for {targetSamples} samples at Fs={fs}. " +
            "Value must be divisible by a standard frame size and require at most 63 frames.");
    }

    /// <summary>
    /// Returns the minimum number of packets the default-configured handler needs
    /// to synchronously evict everything (without relying on the timer).
    /// Accounts for the startup fill phase and one gap recovery cycle.
    /// </summary>
    private static int GetMinPacketCount(int frameSamples)
        => Math.Max(30, (23040 / frameSamples) + 10);

    /// <summary>
    /// Asserts exact contiguous ordered delivery with no losses.
    /// Verifies every field: Ssrc, SequenceNumber, Timestamp, IsLost, DecodeFec, SamplesPerChannel, FecTag.
    /// </summary>
    private static void AssertOrderedDelivery(
        List<ReceivedPacket> packets, ushort startSeq, uint startTimestamp,
        uint samplesPerPacket, int count, uint ssrc = DefaultSsrc)
    {
        Assert.HasCount(count, packets, $"Expected {count} packets, got {packets.Count}.");
        for (int i = 0; i < count; i++)
        {
            var p = packets[i];
            var expectedSeq = (ushort)(startSeq + i);
            var expectedTs = startTimestamp + ((uint)i * samplesPerPacket);

            Assert.AreEqual(ssrc, p.Ssrc, $"SSRC mismatch at index {i}.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Seq mismatch at index {i}: expected {expectedSeq}, got {p.SequenceNumber}.");
            Assert.AreEqual(expectedTs, p.Timestamp, $"Timestamp mismatch at index {i}.");
            Assert.IsFalse(p.IsLost, $"Packet {expectedSeq} should not be lost.");
            Assert.IsFalse(p.DecodeFec, $"Delivered packet {expectedSeq} should not have DecodeFec.");
            Assert.AreEqual(0, p.SamplesPerChannel, $"Delivered packet {expectedSeq} should have SamplesPerChannel=0.");
            Assert.AreEqual(0, p.FecTag, $"Delivered packet {expectedSeq} should have FecTag=0.");
        }
    }

    /// <summary>
    /// Asserts delivery with specified sequences marked as lost.
    /// Consecutive losses are merged into events of up to MaxSamplesPerPacket (5760) samples.
    /// Boundary losses (leading/trailing) are not emitted because the handler lacks context.
    /// Verifies every field including Ssrc, timestamps, SamplesPerChannel, DecodeFec.
    /// </summary>
    private static void AssertSequenceWithLoss(
        List<ReceivedPacket> packets, ushort startSeq, uint startTimestamp,
        uint samplesPerPacket, int totalCount, HashSet<ushort> missedSeqs,
        uint ssrc = DefaultSsrc)
    {
        const int MaxLostSamples = 120 * (48_000 / 1000); // 5760

        // Trim leading/trailing losses (handler can't detect boundary losses).
        int leading = 0;
        while (leading < totalCount && missedSeqs.Contains((ushort)(startSeq + leading)))
            leading++;

        int trailing = 0;
        while (trailing < totalCount - leading && missedSeqs.Contains((ushort)(startSeq + totalCount - 1 - trailing)))
            trailing++;

        int end = totalCount - trailing;
        int idx = 0;
        int i = leading;

        while (i < end)
        {
            var expectedSeq = (ushort)(startSeq + i);
            var expectedTs = startTimestamp + ((uint)i * samplesPerPacket);

            if (!missedSeqs.Contains(expectedSeq))
            {
                Assert.IsTrue(idx < packets.Count,
                    $"Ran out of packets at index {idx}; expected delivered seq {expectedSeq}.");
                var p = packets[idx++];
                Assert.AreEqual(ssrc, p.Ssrc, $"SSRC mismatch at delivered seq {expectedSeq}.");
                Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Seq mismatch at index {idx - 1}.");
                Assert.AreEqual(expectedTs, p.Timestamp, $"Timestamp mismatch at delivered seq {expectedSeq}.");
                Assert.IsFalse(p.IsLost, $"Packet {expectedSeq} should not be lost.");
                Assert.IsFalse(p.DecodeFec, $"Delivered packet {expectedSeq} should not have DecodeFec.");
                Assert.AreEqual(0, p.SamplesPerChannel, $"Delivered packet should have SamplesPerChannel=0.");
                Assert.AreEqual(0, p.FecTag, $"Delivered packet should have FecTag=0.");
                i++;
            }
            else
            {
                // Find consecutive loss range.
                int lossStart = i;
                while (i < end && missedSeqs.Contains((ushort)(startSeq + i)))
                    i++;

                int lossCount = i - lossStart;
                uint totalLostSamples = (uint)lossCount * samplesPerPacket;
                uint lossTs = startTimestamp + ((uint)lossStart * samplesPerPacket);

                // Handler merges consecutive losses into chunks of MaxLostSamples.
                // First chunk: remainder; subsequent: full MaxLostSamples.
                uint firstChunk = totalLostSamples % MaxLostSamples;
                int eventCount = firstChunk == 0
                    ? (int)(totalLostSamples / MaxLostSamples)
                    : 1 + (int)((totalLostSamples - firstChunk) / MaxLostSamples);

                uint currentTs = lossTs;
                for (int e = 0; e < eventCount; e++)
                {
                    Assert.IsTrue(idx < packets.Count,
                        $"Ran out of packets at index {idx}; expected lost event for gap at seq {(ushort)(startSeq + lossStart)}.");
                    var p = packets[idx++];

                    Assert.AreEqual(ssrc, p.Ssrc, $"SSRC mismatch on lost event at index {idx - 1}.");
                    Assert.IsTrue(p.IsLost, $"Expected lost event at index {idx - 1}, got delivered seq {p.SequenceNumber}.");
                    Assert.AreEqual(currentTs, p.Timestamp, $"Lost event timestamp mismatch at index {idx - 1}.");

                    uint expectedSamples = (e == 0 && firstChunk != 0) ? firstChunk : MaxLostSamples;
                    Assert.AreEqual((int)expectedSamples, p.SamplesPerChannel,
                        $"Lost SamplesPerChannel mismatch at index {idx - 1}. Expected {expectedSamples}, got {p.SamplesPerChannel}.");

                    bool isLast = e == eventCount - 1;
                    Assert.AreEqual(isLast, p.DecodeFec,
                        $"Lost DecodeFec should be {isLast} at index {idx - 1}.");

                    // Tag should be 0 for lost events (no frame data).
                    Assert.AreEqual(0, p.Tag, $"Lost event Tag should be 0 at index {idx - 1}.");

                    currentTs += expectedSamples;
                }
            }
        }

        Assert.AreEqual(packets.Count, idx,
            $"Expected {idx} total entries, got {packets.Count}.");
    }

    // ================================================================
    // Configuration Validation
    // ================================================================

    [TestMethod]
    public void DefaultConfiguration_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler();
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void NullConfiguration_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(null);
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void MinimalValidConfiguration_BufferDuration3()
    {
        // BufferDuration=3 → bufferSize = 3*2/5 = 1 (minimum positive)
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void StartupDurationEqualToBufferDuration_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 240,
            StartupDuration = 240,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void ResynchronizationDurationEqualToBufferDuration_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 240,
            ResynchronizationDuration = 240,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void MaxValidResynchronizationDuration_IsValid()
    {
        // 81919 * 2 / 5 = 32767 == short.MaxValue → valid
        var handler = new BufferedVoiceReceiveHandler(new() { ResynchronizationDuration = 81919 });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void MaxValidBufferAndResynchronizationDuration_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 81919,
            ResynchronizationDuration = 81919,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void LargeIdleTimeout_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new() { IdleTimeout = int.MaxValue });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void LargeResynchronizationThreshold_IsValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = int.MaxValue });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void BufferDurationZero_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 0 }));
    }

    [TestMethod]
    public void BufferDurationNegative_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = -1 }));
    }

    [TestMethod]
    public void BufferDurationTooSmall_Throws()
    {
        // Duration 1 or 2 → bufferSize = 0 → invalid
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 1 }));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 2 }));
    }

    [TestMethod]
    public void StartupDurationGreaterThanBuffer_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = 200 }));
    }

    [TestMethod]
    public void ResynchronizationThresholdZeroOrNegative_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = 0 }));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = -5 }));
    }

    [TestMethod]
    public void IdleTimeoutZeroOrNegative_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { IdleTimeout = 0 }));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { IdleTimeout = -1 }));
    }

    [TestMethod]
    public void ResynchronizationDurationTooSmall_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 240, ResynchronizationDuration = 50 }));
    }

    [TestMethod]
    public void ResynchronizationDurationOverflow_Throws()
    {
        // 81920 * 2 / 5 = 32768 > short.MaxValue → throws
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationDuration = 81920 }));
    }

    [TestMethod]
    public void BufferDurationCausesBufferSamplesOverflow_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 44739245, ResynchronizationDuration = 44739245 }));
    }

    [TestMethod]
    public void BufferDurationOverflowViaResynchronization_Throws()
    {
        // BufferDuration=81920 requires ResynchronizationDuration >= 81920 which overflows.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 81920, ResynchronizationDuration = 81920 }));
    }

    // ================================================================
    // Configuration Validation — Extensive Boundary Tests
    // ================================================================

    // --- BufferDuration boundaries ---

    [TestMethod]
    public void BufferDuration_IntMinValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = int.MinValue }));
    }

    [TestMethod]
    public void BufferDuration_IntMaxValue_Throws()
    {
        // int.MaxValue > 2 passes the first check but 2*int.MaxValue overflows int
        // to a negative default ResynchronizationDuration → caught by resync <= 2 check.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = int.MaxValue }));
    }

    [TestMethod]
    public void BufferDuration_4And5_BothValid()
    {
        // bufferDuration=4: bufferSize = 4*2/5 = 1 (same as 3 due to integer division)
        var h4 = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 4,
            StartupDuration = 0,
            ResynchronizationDuration = 4,
        });
        Assert.IsNotNull(h4);

        // bufferDuration=5: bufferSize = 5*2/5 = 2 (first value yielding bufferSize=2)
        var h5 = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 5,
            StartupDuration = 0,
            ResynchronizationDuration = 5,
        });
        Assert.IsNotNull(h5);
    }

    [TestMethod]
    public void BufferDuration_MaxWithDefaultResync_40959Valid_40960Throws()
    {
        // With default ResynchronizationDuration = 2 * BufferDuration:
        //   40959 → resync=81918 → resyncPackets = 81918*2/5 = 32767 = short.MaxValue ✓
        //   40960 → resync=81920 → resyncPackets = 81920*2/5 = 32768 > short.MaxValue ✗
        var handler = new BufferedVoiceReceiveHandler(new() { BufferDuration = 40959 });
        Assert.IsNotNull(handler);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 40960 }));
    }

    [TestMethod]
    public void BufferDuration_81919WithDefaultResync_Throws()
    {
        // BufferDuration=81919 is valid with explicit ResynchronizationDuration=81919,
        // but with the default (2*81919=163838), resyncPackets = 163838*2/5 = 65535 > short.MaxValue → throws.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 81919 }));
    }

    [TestMethod]
    public void BufferDuration_44739244_PassesBufferCheck_FailsResyncCheck()
    {
        // 44739244 is the documented max for the bufferSamples overflow check:
        //   bufferSizeLong = 44739244*2/5 = 17895697 → bufferSamples = 2147483640 ≤ int.MaxValue ✓
        // But no valid ResynchronizationDuration exists (must be ≥ 44739244, max resync is 81919).
        // With resync set to 44739244, it fails the resyncPackets overflow check.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 44739244,
                ResynchronizationDuration = 44739244,
            }));
    }

    [TestMethod]
    public void BufferDuration_44739244_ResyncBelow_Throws()
    {
        // resync=81919 < bufferDuration=44739244 → fails resync < buffer check.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 44739244,
                ResynchronizationDuration = 81919,
            }));
    }

    [TestMethod]
    public void BufferDuration_DefaultResync_IntOverflow_ForVeryLargeBuffer()
    {
        // bufferDuration = int.MaxValue/2 + 1 = 1073741824
        // Default: 2 * 1073741824 overflows int to -2147483648
        // → resynchronizationDuration = -2147483648 ≤ 2 → throws
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 1073741824 }));
    }

    // --- StartupDuration boundaries ---

    [TestMethod]
    public void StartupDuration_ExactLowerBoundary_Minus1Throws_0Valid()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = -1 }));

        var handler = new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = 0 });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void StartupDuration_ExactUpperBoundary_EqualValid_Plus1Throws()
    {
        var handler = new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = 100 });
        Assert.IsNotNull(handler);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = 101 }));
    }

    [TestMethod]
    public void StartupDuration_IntMinValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = int.MinValue }));
    }

    [TestMethod]
    public void StartupDuration_IntMaxValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 100, StartupDuration = int.MaxValue }));
    }

    [TestMethod]
    public void StartupDuration_DefaultWithOddBuffer_Valid()
    {
        // bufferDuration=101 → default startup = 101/2 = 50 (integer division), 50 ≤ 101 ✓
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 101,
            ResynchronizationDuration = 101,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void StartupDuration_DefaultWithMinBuffer_Valid()
    {
        // bufferDuration=3 → default startup = 3/2 = 1, 1 ≤ 3 ✓
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 3,
            ResynchronizationDuration = 3,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void StartupDuration_JustBelowBufferValid_JustAboveBufferThrows()
    {
        // startup = buffer - 1 → valid; startup = buffer + 1 → throws
        var handler = new BufferedVoiceReceiveHandler(new() { BufferDuration = 50, StartupDuration = 49 });
        Assert.IsNotNull(handler);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { BufferDuration = 50, StartupDuration = 51 }));
    }

    // --- ResynchronizationDuration boundaries ---

    [TestMethod]
    public void ResynchronizationDuration_ExactLowerBoundary_2Throws_3Valid()
    {
        // resync=2 with buffer=3 → 2 ≤ 2 → throws
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = 2,
            }));

        // resync=3 with buffer=3 → 3 > 2 and 3 ≥ 3 → valid
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void ResynchronizationDuration_Zero_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = 0,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_One_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = 1,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_Negative_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = -1,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_IntMinValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = int.MinValue,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_IntMaxValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 3,
                StartupDuration = 0,
                ResynchronizationDuration = int.MaxValue,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_LessThanBufferByOne_Throws()
    {
        // Exact boundary: resync = buffer - 1 → resync < buffer → throws
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 100,
                ResynchronizationDuration = 99,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_EqualToBuffer_Valid()
    {
        // Exact boundary: resync = buffer → resync ≥ buffer → valid
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 100,
            ResynchronizationDuration = 100,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void ResynchronizationDuration_OverflowBoundary_81919Valid_81920Throws_WithSmallBuffer()
    {
        // Test the overflow boundary with a small buffer that doesn't constrain resync
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 10,
            ResynchronizationDuration = 81919,
        });
        Assert.IsNotNull(handler);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 10,
                ResynchronizationDuration = 81920,
            }));
    }

    [TestMethod]
    public void ResynchronizationDuration_81921_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 10,
                ResynchronizationDuration = 81921,
            }));
    }

    // --- ResynchronizationThreshold boundaries ---

    [TestMethod]
    public void ResynchronizationThreshold_ExactBoundary_0Throws_1Valid()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = 0 }));

        var handler = new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = 1 });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void ResynchronizationThreshold_Minus1_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = -1 }));
    }

    [TestMethod]
    public void ResynchronizationThreshold_IntMinValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = int.MinValue }));
    }

    [TestMethod]
    public void ResynchronizationThreshold_2_Valid()
    {
        var handler = new BufferedVoiceReceiveHandler(new() { ResynchronizationThreshold = 2 });
        Assert.IsNotNull(handler);
    }

    // --- IdleTimeout boundaries ---

    [TestMethod]
    public void IdleTimeout_ExactBoundary_0Throws_1Valid()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { IdleTimeout = 0 }));

        var handler = new BufferedVoiceReceiveHandler(new() { IdleTimeout = 1 });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void IdleTimeout_Minus1_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { IdleTimeout = -1 }));
    }

    [TestMethod]
    public void IdleTimeout_IntMinValue_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new() { IdleTimeout = int.MinValue }));
    }

    [TestMethod]
    public void IdleTimeout_2_Valid()
    {
        var handler = new BufferedVoiceReceiveHandler(new() { IdleTimeout = 2 });
        Assert.IsNotNull(handler);
    }

    // --- Combined parameter interactions ---

    [TestMethod]
    public void AllParametersAtMinimumValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void AllParametersAtMaximumValid()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 81919,
            StartupDuration = 81919,
            ResynchronizationDuration = 81919,
            ResynchronizationThreshold = int.MaxValue,
            IdleTimeout = int.MaxValue,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void Buffer81920_ResyncLessThanBuffer_Throws()
    {
        // BufferDuration=81920 with resync=81919 → resync < buffer → throws resync error
        // (before the resync overflow check is even reached)
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 81920,
                ResynchronizationDuration = 81919,
            }));
    }

    [TestMethod]
    public void BufferAboveResyncMax_NoValidResyncExists_Throws()
    {
        // BufferDuration=81920 means resync must be ≥ 81920, but 81920 overflows.
        // With resync < buffer → resync check throws.
        // With resync = buffer → overflow check throws.
        // Either way, this configuration is impossible.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 81920,
                ResynchronizationDuration = 81920,
            }));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 81920,
                ResynchronizationDuration = 81919,
            }));
    }

    [TestMethod]
    [DataRow(3)]
    [DataRow(10)]
    [DataRow(100)]
    [DataRow(240)]
    [DataRow(1000)]
    [DataRow(40959)]
    public void BufferDuration_VariousValid_WithExplicitResync(int bufferDuration)
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = bufferDuration,
            StartupDuration = 0,
            ResynchronizationDuration = bufferDuration,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    [DataRow(-1000)]
    [DataRow(-100)]
    [DataRow(-2)]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public void BufferDuration_VariousInvalid_Throws(int bufferDuration)
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = bufferDuration,
                StartupDuration = 0,
                ResynchronizationDuration = Math.Max(3, bufferDuration),
            }));
    }

    [TestMethod]
    [DataRow(0, true)]
    [DataRow(1, true)]
    [DataRow(50, true)]
    [DataRow(99, true)]
    [DataRow(100, true)]
    [DataRow(-1, false)]
    [DataRow(101, false)]
    [DataRow(200, false)]
    public void StartupDuration_Various_WithBuffer100(int startupDuration, bool shouldSucceed)
    {
        if (shouldSucceed)
        {
            var handler = new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 100,
                StartupDuration = startupDuration,
            });
            Assert.IsNotNull(handler);
        }
        else
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new BufferedVoiceReceiveHandler(new()
                {
                    BufferDuration = 100,
                    StartupDuration = startupDuration,
                }));
        }
    }

    [TestMethod]
    [DataRow(100, true)]
    [DataRow(200, true)]
    [DataRow(81919, true)]
    [DataRow(99, false)]
    [DataRow(50, false)]
    [DataRow(3, false)]
    [DataRow(2, false)]
    [DataRow(1, false)]
    [DataRow(0, false)]
    [DataRow(-1, false)]
    [DataRow(81920, false)]
    [DataRow(81921, false)]
    public void ResynchronizationDuration_Various_WithBuffer100(int resyncDuration, bool shouldSucceed)
    {
        if (shouldSucceed)
        {
            var handler = new BufferedVoiceReceiveHandler(new()
            {
                BufferDuration = 100,
                ResynchronizationDuration = resyncDuration,
            });
            Assert.IsNotNull(handler);
        }
        else
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new BufferedVoiceReceiveHandler(new()
                {
                    BufferDuration = 100,
                    ResynchronizationDuration = resyncDuration,
                }));
        }
    }

    // ================================================================
    // API / Properties
    // ================================================================

    [TestMethod]
    public void RequiresExternalSocketAddress_IsTrue()
    {
        var handler = new BufferedVoiceReceiveHandler();
        Assert.IsTrue(handler.RequiresExternalSocketAddress);
    }

    [TestMethod]
    public void NoEventHandler_DoesNotCrash()
    {
        var handler = new BufferedVoiceReceiveHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }
    }

    // ================================================================
    // Ordered Delivery at Various Frame Sizes
    // ================================================================

    [TestMethod]
    [DataRow(120)]      // 2.5ms - minimum
    [DataRow(240)]      // 5ms
    [DataRow(360)]      // non-standard: 3 × 120 (CELT)
    [DataRow(480)]      // 10ms
    [DataRow(600)]      // non-standard: 5 × 120 (CELT)
    [DataRow(720)]      // non-standard: 6 × 120 (CELT)
    [DataRow(960)]      // 20ms - most common
    [DataRow(1080)]     // non-standard: 9 × 120 (CELT)
    [DataRow(1440)]     // non-standard: 3 × 480 (SILK)
    [DataRow(1920)]     // 40ms
    [DataRow(2400)]     // non-standard: 5 × 480 (SILK)
    [DataRow(2880)]     // 60ms
    [DataRow(3600)]     // non-standard: 30 × 120 (CELT)
    [DataRow(5760)]     // 120ms - maximum
    public void OrderedDelivery_AllFrameSizes(int frameSamples)
    {
        var spp = (uint)frameSamples;
        int packetCount = GetMinPacketCount(frameSamples);
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= packetCount; i++)
        {
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += spp;
        }

        AssertOrderedDelivery(packets, 1, 10000, spp, packetCount);
    }

    [TestMethod]
    [DataRow(120)]
    [DataRow(360)]      // non-standard
    [DataRow(600)]      // non-standard
    [DataRow(960)]
    [DataRow(1080)]     // non-standard
    [DataRow(1440)]     // non-standard
    [DataRow(1920)]
    [DataRow(2880)]
    [DataRow(3600)]     // non-standard
    [DataRow(5760)]
    public void OrderedDelivery_WithTagVerification(int frameSamples)
    {
        var spp = (uint)frameSamples;
        int packetCount = GetMinPacketCount(frameSamples);
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= packetCount; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i, frameSamples: frameSamples));
            timestamp += spp;
        }

        Assert.HasCount(packetCount, packets);
        for (int i = 0; i < packetCount; i++)
        {
            var p = packets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(10000u + ((uint)i * spp), p.Timestamp);
            Assert.IsFalse(p.IsLost);
            Assert.AreEqual(expectedSeq, p.Tag, $"Tag mismatch at seq {expectedSeq}.");
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
            Assert.AreEqual(0, p.FecTag);
        }
    }

    // ================================================================
    // Reordering
    // ================================================================

    [TestMethod]
    public void ReorderedPackets_DeliveredInOrder()
    {
        var (handler, packets) = CreateHandler();

        List<ushort> sendOrder = [1, 3, 2, 4, 6, 5, 7, 8, 9, 10, 12, 11];
        for (ushort i = 13; i <= 30; i++)
            sendOrder.Add(i);

        uint timestampBase = 10000;
        foreach (var seq in sendOrder)
            handler.Handle(Packet(seq, timestampBase + ((uint)(seq - 1) * DefaultFrameSamples)));

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void AlternatingPairSwap_DeliveredInOrder()
    {
        var (handler, packets) = CreateHandler();

        uint timestampBase = 10000;
        List<ushort> sendOrder = [];
        for (ushort i = 1; i <= 30; i += 2)
        {
            ushort next = (ushort)(i + 1);
            if (next <= 30) sendOrder.Add(next);
            sendOrder.Add(i);
        }

        foreach (var seq in sendOrder)
            handler.Handle(Packet(seq, timestampBase + ((uint)(seq - 1) * DefaultFrameSamples)));

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    [DataRow(360)]      // non-standard
    [DataRow(1080)]     // non-standard
    [DataRow(2400)]     // non-standard
    public void ReorderedPackets_NonStandardFrameSize(int frameSamples)
    {
        var spp = (uint)frameSamples;
        var (handler, packets) = CreateHandler();

        // Reorder: 1, 3, 2, 4-30
        uint timestampBase = 10000;
        handler.Handle(Packet(1, timestampBase, frameSamples: frameSamples));
        handler.Handle(Packet(3, timestampBase + (2 * spp), frameSamples: frameSamples));
        handler.Handle(Packet(2, timestampBase + spp, frameSamples: frameSamples));

        for (ushort i = 4; i <= 30; i++)
            handler.Handle(Packet(i, timestampBase + ((uint)(i - 1) * spp), frameSamples: frameSamples));

        AssertOrderedDelivery(packets, 1, 10000, spp, 30);
    }

    [TestMethod]
    public void CompletelyReversedPackets_DoesNotCrash()
    {
        var (handler, packets) = CreateHandler();
        uint timestampBase = 10000;

        for (ushort i = 30; i >= 1; i--)
            handler.Handle(Packet(i, timestampBase + ((uint)(i - 1) * DefaultFrameSamples)));

        Assert.IsNotEmpty(packets);
        var nonMissed = packets.Where(p => !p.IsLost).ToList();
        Assert.IsNotEmpty(nonMissed, "Some packets should be delivered even with full reversal.");
        // Verify SSRC is correct on all emitted packets
        foreach (var p in packets)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
    }

    [TestMethod]
    public void LatePacketWithinBufferWindow_IsDelivered()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // Send 1, 2, 4, 5 (skip 3), then send 3 late
        handler.Handle(Packet(1, ts));
        handler.Handle(Packet(2, ts + DefaultFrameSamples));
        handler.Handle(Packet(4, ts + (3 * DefaultFrameSamples)));
        handler.Handle(Packet(5, ts + (4 * DefaultFrameSamples)));
        handler.Handle(Packet(3, ts + (2 * DefaultFrameSamples))); // Late arrival

        for (ushort i = 6; i <= 30; i++)
            handler.Handle(Packet(i, ts + ((uint)(i - 1) * DefaultFrameSamples)));

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task RandomJitter_AllDelivered()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;
        var rng = new Random(42);

        List<ushort> seqs = [];
        for (ushort i = 1; i <= 200; i++)
            seqs.Add(i);

        // Shuffle with limited displacement (realistic network jitter)
        for (int i = 0; i < seqs.Count; i++)
        {
            int target = Math.Clamp(i + rng.Next(-2, 3), 0, seqs.Count - 1);
            (seqs[i], seqs[target]) = (seqs[target], seqs[i]);
        }

        foreach (var seq in seqs)
            handler.Handle(Packet(seq, ts + ((uint)(seq - 1) * DefaultFrameSamples)));

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 200);
    }

    [TestMethod]
    public void BurstyArrival_DeliveredInOrder()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // Packets arrive in groups of 5
        for (int burst = 0; burst < 6; burst++)
        {
            for (int j = 0; j < 5; j++)
            {
                ushort seq = (ushort)((burst * 5) + j + 1);
                handler.Handle(Packet(seq, ts + ((uint)(seq - 1) * DefaultFrameSamples)));
            }
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    // ================================================================
    // Missing Packets / Loss
    // ================================================================

    [TestMethod]
    [DataRow(120)]
    [DataRow(360)]      // non-standard
    [DataRow(600)]      // non-standard
    [DataRow(960)]
    [DataRow(1080)]     // non-standard
    [DataRow(1440)]     // non-standard
    [DataRow(1920)]
    [DataRow(2880)]
    [DataRow(3600)]     // non-standard
    [DataRow(5760)]
    public void SingleMissingPacket_ReportedAsLost(int frameSamples)
    {
        var spp = (uint)frameSamples;
        int packetCount = GetMinPacketCount(frameSamples);
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= packetCount; i++)
        {
            if (i == 5)
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += spp;
        }

        AssertSequenceWithLoss(packets, 1, 10000, spp, packetCount, new HashSet<ushort> { 5 });
    }

    [TestMethod]
    public void MultipleConsecutiveMissing_MergedIntoOneLostEvent()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            if (i is >= 5 and <= 7)
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 30, new HashSet<ushort> { 5, 6, 7 });
    }

    [TestMethod]
    public void BurstLossAndRecovery()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Burst loss: skip 11-20
        timestamp += 10 * DefaultFrameSamples;

        for (ushort i = 21; i <= 40; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 40,
            new HashSet<ushort> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
    }

    [TestMethod]
    public void MultipleScatteredGaps_EachProducesLostEvents()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        HashSet<ushort> skipped = [5, 15, 25];

        for (ushort i = 1; i <= 40; i++)
        {
            if (skipped.Contains(i))
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 40, skipped);
    }

    [TestMethod]
    public void MissingAndReorderedCombined()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // Pattern: 1, 3, 2, (skip 4), 6, 5, 7, 8, (skip 9), 11, 10, 12-30
        List<ushort> sendOrder = [1, 3, 2, 6, 5, 7, 8, 11, 10];
        for (ushort i = 12; i <= 30; i++) sendOrder.Add(i);

        foreach (var seq in sendOrder)
            handler.Handle(Packet(seq, ts + ((uint)(seq - 1) * DefaultFrameSamples)));

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 30, new HashSet<ushort> { 4, 9 });
    }

    [TestMethod]
    [DataRow(360)]      // non-standard: 5 missing × 360 = 1800 samples (< MaxLostSamples)
    [DataRow(1080)]     // non-standard: 5 missing × 1080 = 5400 samples (< MaxLostSamples)
    [DataRow(1440)]     // non-standard: 5 missing × 1440 = 7200 samples (split: 1440 + 5760)
    [DataRow(2400)]     // non-standard: 5 missing × 2400 = 12000 (split: 480 + 5760 + 5760)
    public void BurstLoss_NonStandardFrameSize_ProperMerging(int frameSamples)
    {
        var spp = (uint)frameSamples;
        int packetCount = GetMinPacketCount(frameSamples);
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= packetCount; i++)
        {
            if (i is >= 6 and <= 10)
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += spp;
        }

        AssertSequenceWithLoss(packets, 1, 10000, spp, packetCount,
            new HashSet<ushort> { 6, 7, 8, 9, 10 });
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task RandomPacketLoss10Percent()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        var rng = new Random(123);
        HashSet<ushort> dropped = [];

        for (ushort i = 1; i <= 200; i++)
        {
            if (rng.NextDouble() < 0.1)
            {
                dropped.Add(i);
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 200, dropped);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task RandomPacketLoss50Percent()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        var rng = new Random(999);
        HashSet<ushort> dropped = [];

        for (ushort i = 1; i <= 200; i++)
        {
            if (rng.NextDouble() < 0.5)
            {
                dropped.Add(i);
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 200, dropped);
    }

    [TestMethod]
    public void SparsePackets_EveryFifth()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;
        var sent = new HashSet<ushort> { 1, 6, 11, 16, 21, 26 };

        foreach (var seq in sent)
            handler.Handle(Packet(seq, ts + ((uint)(seq - 1) * DefaultFrameSamples)));

        Assert.IsNotEmpty(packets);
        foreach (var p in packets.Where(p => !p.IsLost))
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.IsTrue(sent.Contains(p.SequenceNumber), $"Unexpected seq {p.SequenceNumber}.");
        }
    }

    // ================================================================
    // Duplicate Packets
    // ================================================================

    [TestMethod]
    public void DuplicatePackets_Ignored()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            if (i % 5 == 0)
                handler.Handle(Packet(i, timestamp)); // Duplicate
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void ConsecutiveTriplicates_DeliveredOnce()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            handler.Handle(Packet(i, timestamp));
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void DuplicateOfLatestPacket_OriginalDataPreserved()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        handler.Handle(Packet(1, timestamp, tag: 42));
        handler.Handle(Packet(1, timestamp, tag: 99)); // Duplicate with different tag

        timestamp += DefaultFrameSamples;
        for (ushort i = 2; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        var seq1 = packets.First(p => !p.IsLost && p.SequenceNumber == 1);
        Assert.AreEqual(42, seq1.Tag, "Original tag should be preserved, not the duplicate's.");
    }

    [TestMethod]
    public void DuplicateAfterInterveningPackets_OriginalDataPreserved()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort batch = 0; batch < 6; batch++)
        {
            ushort batchStart = (ushort)((batch * 5) + 1);
            for (ushort j = 0; j < 5; j++)
            {
                ushort seq = (ushort)(batchStart + j);
                handler.Handle(Packet(seq, timestamp, tag: seq));
                timestamp += DefaultFrameSamples;
            }
            // Duplicate of batchStart with different tag
            handler.Handle(Packet(batchStart, timestamp - (5 * DefaultFrameSamples), tag: batchStart + 10000));
        }

        var nonMissed = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(30, nonMissed);
        foreach (var p in nonMissed)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(p.SequenceNumber, p.Tag,
                $"Seq {p.SequenceNumber}: tag should be {p.SequenceNumber} (original), got {p.Tag}.");
        }
    }

    [TestMethod]
    public void DelayedDuplicateAfterEviction_OriginalDataPreserved()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        // Late duplicates of already-evicted packets
        handler.Handle(Packet(1, 10000, tag: 9001));
        handler.Handle(Packet(2, 10000 + DefaultFrameSamples, tag: 9002));

        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        for (ushort seq = 1; seq <= 2; seq++)
        {
            var p = packets.First(pkt => !pkt.IsLost && pkt.SequenceNumber == seq);
            Assert.AreEqual(seq, p.Tag, $"Seq {seq} should retain original tag, not delayed duplicate's.");
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void VeryLatePacket_NoDuplicateDelivery()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 100; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        handler.Handle(Packet(2, 10000 + DefaultFrameSamples)); // Very late

        var seq2Count = packets.Count(p => !p.IsLost && p.SequenceNumber == 2);
        Assert.AreEqual(1, seq2Count, "Late packet should not cause duplicate delivery.");
    }

    // ================================================================
    // Sequence Number Wraparound (ushort boundary)
    // ================================================================

    [TestMethod]
    public void SequenceWraparound_OrderedDelivery()
    {
        var (handler, packets) = CreateHandler();
        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            handler.Handle(Packet((ushort)(startSeq + i), timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, startSeq, 10000, DefaultFrameSamples, 32);
    }

    [TestMethod]
    public void SequenceWraparound_WithReordering()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // Out of order near wraparound
        List<ushort> sendOrder = [65534, 0, 65535, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
            13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];

        foreach (var seq in sendOrder)
        {
            var seqOffset = (ushort)(seq - 65534);
            handler.Handle(Packet(seq, ts + (seqOffset * DefaultFrameSamples)));
        }

        AssertOrderedDelivery(packets, 65534, 10000, DefaultFrameSamples, 28);
    }

    [TestMethod]
    public void SequenceWraparound_MissingPacketAt65535()
    {
        var (handler, packets) = CreateHandler();
        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            if (seq == 65535)
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(seq, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertSequenceWithLoss(packets, startSeq, 10000, DefaultFrameSamples, 32, new HashSet<ushort> { 65535 });
    }

    [TestMethod]
    public void SequenceWraparound_MissingPacketAtZero()
    {
        var (handler, packets) = CreateHandler();
        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            if (seq == 0)
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(seq, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertSequenceWithLoss(packets, startSeq, 10000, DefaultFrameSamples, 32, new HashSet<ushort> { 0 });
    }

    // ================================================================
    // Timestamp Wraparound (uint boundary)
    // ================================================================

    [TestMethod]
    public void TimestampWraparound_OrderedDelivery()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = uint.MaxValue - (10 * DefaultFrameSamples);

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, uint.MaxValue - (10 * DefaultFrameSamples), DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void TimestampStartsAtZero_OrderedDelivery()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 0;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 0, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void BothSequenceAndTimestampWraparound()
    {
        var (handler, packets) = CreateHandler();
        ushort startSeq = 65530;
        uint timestamp = uint.MaxValue - (5 * DefaultFrameSamples);

        for (int i = 0; i < 30; i++)
        {
            handler.Handle(Packet((ushort)(startSeq + i), timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, startSeq, uint.MaxValue - (5 * DefaultFrameSamples), DefaultFrameSamples, 30);
    }

    // ================================================================
    // Multiple SSRCs
    // ================================================================

    [TestMethod]
    public void MultipleSSRCs_IndependentDelivery()
    {
        var (handler, packets) = CreateHandler();
        uint ts1 = 10000, ts2 = 20000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, ts1, ssrc: 1000, tag: i));
            handler.Handle(Packet(i, ts2, ssrc: 2000, tag: i + 100));
            ts1 += DefaultFrameSamples;
            ts2 += DefaultFrameSamples;
        }

        var ssrc1 = packets.Where(p => p.Ssrc == 1000 && !p.IsLost).ToList();
        var ssrc2 = packets.Where(p => p.Ssrc == 2000 && !p.IsLost).ToList();

        Assert.IsNotEmpty(ssrc1);
        Assert.IsNotEmpty(ssrc2);

        // Verify each stream is ordered
        for (int i = 1; i < ssrc1.Count; i++)
            Assert.IsGreaterThan(ssrc1[i - 1].SequenceNumber, ssrc1[i].SequenceNumber);
        for (int i = 1; i < ssrc2.Count; i++)
            Assert.IsGreaterThan(ssrc2[i - 1].SequenceNumber, ssrc2[i].SequenceNumber);

        // Verify SSRC isolation (no cross-contamination of tags)
        foreach (var p in ssrc1)
            Assert.IsTrue(p.Tag is >= 1 and <= 30, $"SSRC 1000 has unexpected tag {p.Tag}.");
        foreach (var p in ssrc2)
            Assert.IsTrue(p.Tag is >= 101 and <= 130, $"SSRC 2000 has unexpected tag {p.Tag}.");
    }

    [TestMethod]
    public void SSRCZero_HandledCorrectly()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp, ssrc: 0));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30, ssrc: 0);
    }

    [TestMethod]
    public void SSRCMaxValue_HandledCorrectly()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp, ssrc: uint.MaxValue));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30, ssrc: uint.MaxValue);
    }

    [TestMethod]
    public void MultipleSSRCs_OneLossy()
    {
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // SSRC 100: ordered, complete
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, ts, ssrc: 100));
            ts += DefaultFrameSamples;
        }

        // SSRC 200: missing seq 5
        ts = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 5) { ts += DefaultFrameSamples; continue; }
            handler.Handle(Packet(i, ts, ssrc: 200));
            ts += DefaultFrameSamples;
        }

        var ssrc200Seq5 = packets.FirstOrDefault(p => p.Ssrc == 200 && p.SequenceNumber == 5);
        Assert.IsTrue(ssrc200Seq5.IsLost, "SSRC 200: seq 5 should be lost.");
        Assert.AreEqual(200u, ssrc200Seq5.Ssrc, "Lost event SSRC should be 200.");
    }

    // ================================================================
    // Outlier Tracking / Resynchronization
    // ================================================================

    [TestMethod]
    public void OutliersBelowThreshold_NoResync()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 5 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // 4 contiguous outliers (below threshold of 5)
        for (int i = 0; i < 4; i++)
            handler.Handle(Packet((ushort)(30000 + i), 5000000u + (uint)(i * DefaultFrameSamples)));

        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void OutlierCounterReset_NonContiguousOutliers()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 3 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Two outliers from one range
        handler.Handle(Packet(30000, 5000000));
        handler.Handle(Packet(30001, 5000000 + DefaultFrameSamples));

        // Non-contiguous outlier (resets counter)
        handler.Handle(Packet(50000, 9000000));
        handler.Handle(Packet(50001, 9000000 + DefaultFrameSamples));

        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        var normalDelivered = packets.Where(p => !p.IsLost && p.SequenceNumber is >= 1 and <= 30).ToList();
        Assert.HasCount(30, normalDelivered, "Normal flow should continue despite non-contiguous outliers.");
    }

    [TestMethod]
    public void ExactThreshold_TriggersResync()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 3 });
        uint timestamp = 10000;

        handler.Handle(Packet(1, timestamp));
        handler.Handle(Packet(2, timestamp + DefaultFrameSamples));

        // Exactly 3 contiguous outliers → triggers resync
        ushort jumpBase = 5000;
        uint jumpTs = 10000000;
        handler.Handle(Packet(jumpBase, jumpTs));
        handler.Handle(Packet((ushort)(jumpBase + 1), jumpTs + DefaultFrameSamples));
        handler.Handle(Packet((ushort)(jumpBase + 2), jumpTs + (2 * DefaultFrameSamples)));

        for (ushort i = 3; i <= 25; i++)
            handler.Handle(Packet((ushort)(jumpBase + i), jumpTs + (i * DefaultFrameSamples)));

        // Old packets force-evicted
        var seq1 = packets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.IsLost, "Seq 1 should be force-evicted during resync.");
        Assert.AreEqual(DefaultSsrc, seq1.Ssrc);

        var seq2 = packets.FirstOrDefault(p => p.SequenceNumber == 2);
        Assert.IsFalse(seq2.IsLost, "Seq 2 should be force-evicted during resync.");

        var newRange = packets.Where(p => !p.IsLost && p.SequenceNumber >= jumpBase).ToList();
        Assert.IsNotEmpty(newRange, "New range should be delivered after resync.");
    }

    [TestMethod]
    public void BelowThresholdByOne_NoResync()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 4 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // 3 outliers (threshold is 4 → no resync)
        for (int i = 0; i < 3; i++)
            handler.Handle(Packet((ushort)(30000 + i), 5000000u + (uint)(i * DefaultFrameSamples)));

        for (ushort i = 6; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        var normalDelivered = packets.Where(p => !p.IsLost && p.SequenceNumber is >= 1 and <= 30).ToList();
        Assert.IsNotEmpty(normalDelivered, "Normal flow should continue when below threshold.");
    }

    [TestMethod]
    public void ResyncThresholdOne_SingleOutlierTriggersResync()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp));
        handler.Handle(Packet(2, timestamp + DefaultFrameSamples));

        // Single outlier → triggers resync
        ushort jumpSeq = 500;
        uint jumpTs = 5000000;
        handler.Handle(Packet(jumpSeq, jumpTs));

        for (ushort i = 1; i <= 25; i++)
            handler.Handle(Packet((ushort)(jumpSeq + i), jumpTs + (i * DefaultFrameSamples)));

        var seq1 = packets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.IsLost, "Seq 1 force-evicted during resync.");

        var postJump = packets.Where(p => !p.IsLost && p.SequenceNumber >= jumpSeq).ToList();
        Assert.IsNotEmpty(postJump, "Post-resync packets should be delivered.");
    }

    [TestMethod]
    public void DoubleResynchronization()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 2 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // First jump
        ushort jump1 = 10000;
        uint jump1Ts = 20000000;
        handler.Handle(Packet(jump1, jump1Ts));
        handler.Handle(Packet((ushort)(jump1 + 1), jump1Ts + DefaultFrameSamples));

        for (ushort i = 2; i <= 5; i++)
            handler.Handle(Packet((ushort)(jump1 + i), jump1Ts + (i * DefaultFrameSamples)));

        // Second jump
        ushort jump2 = 50000;
        uint jump2Ts = 80000000;
        handler.Handle(Packet(jump2, jump2Ts));
        handler.Handle(Packet((ushort)(jump2 + 1), jump2Ts + DefaultFrameSamples));

        for (ushort i = 2; i <= 25; i++)
            handler.Handle(Packet((ushort)(jump2 + i), jump2Ts + (i * DefaultFrameSamples)));

        var range1 = packets.Where(p => !p.IsLost && p.SequenceNumber is >= 1 and <= 5).ToList();
        var range2 = packets.Where(p => !p.IsLost && p.SequenceNumber >= jump1 && p.SequenceNumber <= jump1 + 5).ToList();
        var range3 = packets.Where(p => !p.IsLost && p.SequenceNumber >= jump2).ToList();

        Assert.HasCount(5, range1, "First range should be force-evicted.");
        Assert.IsNotEmpty(range2, "Second range should be force-evicted.");
        Assert.IsNotEmpty(range3, "Third range should be delivered.");
    }

    [TestMethod]
    public void ResyncAcrossSequenceWraparound()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 2 });
        uint timestamp = 10000;

        ushort startSeq = 65530;
        for (int i = 0; i < 5; i++)
        {
            handler.Handle(Packet((ushort)(startSeq + i), timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Jump across wraparound
        ushort jumpSeq = 100;
        uint jumpTs = 50000000;
        handler.Handle(Packet(jumpSeq, jumpTs));
        handler.Handle(Packet((ushort)(jumpSeq + 1), jumpTs + DefaultFrameSamples));

        for (ushort i = 2; i <= 25; i++)
            handler.Handle(Packet((ushort)(jumpSeq + i), jumpTs + (i * DefaultFrameSamples)));

        var preJump = packets.Where(p => !p.IsLost && p.SequenceNumber >= startSeq).ToList();
        Assert.HasCount(5, preJump, "Pre-jump packets should be force-evicted.");

        var postJump = packets.Where(p => !p.IsLost && p.SequenceNumber >= jumpSeq && p.SequenceNumber < jumpSeq + 26).ToList();
        Assert.IsNotEmpty(postJump, "Post-resync packets should be delivered.");
    }

    [TestMethod]
    public void ResynchronizationWithNonStandardFrameSize()
    {
        const int frameSamples = 1080; // non-standard: 9 × 120
        var spp = (uint)frameSamples;
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 3 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += spp;
        }

        // 3 contiguous outliers → triggers resync
        ushort jumpBase = 500;
        uint jumpTs = 5000000;
        for (int i = 0; i < 3; i++)
            handler.Handle(Packet((ushort)(jumpBase + i), jumpTs + ((uint)i * spp), frameSamples: frameSamples));

        for (ushort i = 3; i <= 25; i++)
            handler.Handle(Packet((ushort)(jumpBase + i), jumpTs + (i * spp), frameSamples: frameSamples));

        var range1 = packets.Where(p => !p.IsLost && p.SequenceNumber is >= 1 and <= 5).ToList();
        Assert.HasCount(5, range1, "Original range force-evicted during resync.");

        var newRange = packets.Where(p => !p.IsLost && p.SequenceNumber >= jumpBase).ToList();
        Assert.IsNotEmpty(newRange, "New range delivered after resync.");
    }

    // ================================================================
    // Timestamp Inconsistencies
    // ================================================================

    [TestMethod]
    public async Task InconsistentTimestampGaps_TreatedAsOutliers()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples + i; // Non-aligned increment
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        Assert.HasCount(1, packets,
            "With inconsistent timestamps, only the first packet is accepted; rest are outliers.");
        Assert.AreEqual(DefaultSsrc, packets[0].Ssrc);
        Assert.IsFalse(packets[0].IsLost);
    }

    [TestMethod]
    public void InconsistentTimestampPackets_IgnoredByNormalStream()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 100 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Packets with timestamps misaligned from 120-sample boundary → outliers
        // latestTs = 10000 + 9*960 = 18640. 18640 + 961 → 961 % 120 = 1 ≠ 0
        handler.Handle(Packet(11, 18640 + 961));
        handler.Handle(Packet(12, 18640 + 961 + DefaultFrameSamples));

        // Resume correctly-aligned stream
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void InconsistentTimestampOutliers_TriggerResyncAtThreshold()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 3 });
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // 3 outliers consistent with each other but not with main stream → resync
        uint outlierBase = 5000000;
        handler.Handle(Packet(500, outlierBase));
        handler.Handle(Packet(501, outlierBase + DefaultFrameSamples));
        handler.Handle(Packet(502, outlierBase + (2 * DefaultFrameSamples)));

        for (ushort i = 3; i <= 25; i++)
            handler.Handle(Packet((ushort)(500 + i), outlierBase + (i * DefaultFrameSamples)));

        var seq1 = packets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.IsLost, "Seq 1 should be force-evicted during resync.");

        var newRange = packets.Where(p => !p.IsLost && p.SequenceNumber >= 500).ToList();
        Assert.IsNotEmpty(newRange, "New range delivered after resync.");
    }

    // ================================================================
    // Timestamp Increment Smaller Than Frame Size
    // ================================================================
    //
    // In these tests the Opus TOC byte encodes a frame size (e.g. 960 samples)
    // but the RTP timestamp increment between consecutive packets is smaller
    // than that frame size.  This can happen with real-world encoders that use
    // overlapping analysis windows, or when servers mis-stamp packets.
    //
    // Without packet loss the handler delivers every packet in order.
    // When loss occurs, the gap calculator uses the Opus frame size (from the
    // TOC) to compute the lost-audio start point:
    //   lostTimestamp = lastEvictedTs + lastEvictedPacketSamples
    // If that already reaches or overshoots the next delivered packet's timestamp
    // (i.e. timestampDiff <= 0), the gap is fully covered by the previous Opus
    // frame and NO lost event is emitted.  A lost event is only emitted for the
    // uncovered portion: when (N+1)*tsIncrement > frameSize, where N is the
    // number of consecutive missing packets.

    [TestMethod]
    public void TimestampIncrementHalfFrameSize_NoLoss_AllDelivered()
    {
        // Opus frame = 960 samples, RTP timestamp advances by only 480.
        // No packet loss → handler should deliver all packets in order.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        Assert.HasCount(50, packets);
        for (int i = 0; i < 50; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(10000u + ((uint)i * 480), p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [TestMethod]
    public void TimestampIncrementMinimum120_FrameSize960_NoLoss_AllDelivered()
    {
        // Minimum valid aligned increment (120) with 960-sample Opus frames.
        // This is the most extreme mismatch (8× difference).
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        Assert.HasCount(50, packets);
        for (int i = 0; i < 50; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(10000u + ((uint)i * 120), p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [TestMethod]
    public void TimestampIncrementHalfFrameSize_SingleMissing_NoLostEvent()
    {
        // Opus frame = 960 samples, RTP timestamps advance by 480. Seq 5 missing.
        // lostTimestamp = ts_4 + 960, nextTs = ts_4 + 2*480 = ts_4 + 960.
        // timestampDiff = 0 → gap is fully covered by previous frame → no lost event.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i == 5)
            {
                timestamp += 480;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(49, delivered, "49 of 50 packets should be delivered.");

        foreach (var p in delivered)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(10000u + ((uint)(p.SequenceNumber - 1) * 480), p.Timestamp,
                $"Timestamp mismatch at delivered seq {p.SequenceNumber}.");
        }

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "Single missing with ts increment = frame/2: previous frame already covers the gap.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrementMinimum120_FrameSize960_SingleMissing_NoLostEvent()
    {
        // 960-sample Opus frames with 120-sample RTP timestamp increments.
        // Seq 5 missing. lostTimestamp = ts_4 + 960, nextTs = ts_4 + 240.
        // timestampDiff = 240 - 960 = -720 → gap fully covered → no lost event.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i == 5)
            {
                timestamp += 120;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(49, delivered, "49 of 50 packets should be delivered.");

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "Single missing with ts increment = frame/8: previous frame already covers the gap.");
    }

    [TestMethod]
    public void TimestampIncrementHalfFrameSize_ConsecutiveMissing_UncoveredGap()
    {
        // 960-sample frames, 480-sample ts increment. Seq 5-7 missing (3 consecutive).
        // lostTimestamp = ts_4 + 960, nextTs (seq 8) = ts_4 + 4*480 = ts_4 + 1920.
        // timestampDiff = 1920 - 960 = 960 > 0 → lost event with 960 samples.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i is >= 5 and <= 7)
            {
                timestamp += 480;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(47, delivered, "47 of 50 packets should be delivered.");

        foreach (var p in delivered)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(10000u + ((uint)(p.SequenceNumber - 1) * 480), p.Timestamp,
                $"Timestamp mismatch at seq {p.SequenceNumber}.");
        }

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsNotEmpty(lost,
            "3 consecutive missing × 480 ts increment = 1440 > 960 frame → uncovered gap exists.");
        foreach (var l in lost)
        {
            Assert.AreEqual(DefaultSsrc, l.Ssrc);
            Assert.IsGreaterThan(0, l.SamplesPerChannel,
                "SamplesPerChannel must be positive for the uncovered portion.");
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrementMinimum120_FrameSize960_ConsecutiveMissing_FullyCovered()
    {
        // 960-sample frames, 120-sample ts increment. Seq 5-7 missing (3 consecutive).
        // lostTimestamp = ts_4 + 960, nextTs (seq 8) = ts_4 + 4*120 = ts_4 + 480.
        // timestampDiff = 480 - 960 = -480 → gap fully covered → no lost event.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i is >= 5 and <= 7)
            {
                timestamp += 120;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(47, delivered, "47 of 50 packets should be delivered.");

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "3 missing × 120 = 360. Frame size 960 covers 360 → no gap, no lost event.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrement120_FrameSize960_ExactBoundary_7Missing_NoLostEvent()
    {
        // Edge case: exactly at boundary. 7 missing, lostTs = ts_4 + 960,
        // nextTs (seq 12) = ts_4 + 8*120 = ts_4 + 960. timestampDiff = 0 → no lost event.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i is >= 5 and <= 11)
            {
                timestamp += 120;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(43, delivered, "43 of 50 packets should be delivered.");

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "7 missing × 120 = 840. lostTs = evictedTs+960. nextTs = evictedTs+960. " +
            "timestampDiff = 0 → no lost event.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrement120_FrameSize960_8Missing_ProducesLostEvent()
    {
        // Just past boundary. 8 missing, lostTs = ts_4 + 960, nextTs = ts_4 + 9*120 = ts_4 + 1080.
        // timestampDiff = 1080 - 960 = 120 → lost event with 120 samples.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i is >= 5 and <= 12)
            {
                timestamp += 120;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(42, delivered, "42 of 50 packets should be delivered.");

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.HasCount(1, lost, "8 missing × 120 exceeds 960 frame → 1 lost event.");
        Assert.AreEqual(120, lost[0].SamplesPerChannel,
            "Uncovered portion is (9*120 - 960) = 120 samples.");
        Assert.AreEqual(DefaultSsrc, lost[0].Ssrc);
    }

    [TestMethod]
    public void TimestampIncrementHalfFrameSize_Reordered_NoLoss()
    {
        // Timestamp increment < frame size combined with reordering, no loss.
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        // Send: 1, 3, 2, 4-50
        handler.Handle(Packet(1, ts, tag: 1));
        handler.Handle(Packet(3, ts + (2 * 480), tag: 3));
        handler.Handle(Packet(2, ts + 480, tag: 2));

        for (ushort i = 4; i <= 50; i++)
            handler.Handle(Packet(i, ts + ((uint)(i - 1) * 480), tag: i));

        Assert.HasCount(50, packets);
        for (int i = 0; i < 50; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(10000u + ((uint)i * 480), p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [TestMethod]
    public void TimestampIncrementHalfFrameSize_Reordered_WithMissing_NoLostEvent()
    {
        // Reordered + single missing packet with timestamp/frame-size mismatch.
        // Seq: 1, 3, 2, (skip 4), 5, 7, 6, 8-50.
        // Single missing → gap covered by previous frame → no lost event.
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        List<ushort> sendOrder = [1, 3, 2, 5, 7, 6];
        for (ushort i = 8; i <= 50; i++)
            sendOrder.Add(i);

        foreach (var seq in sendOrder)
            handler.Handle(Packet(seq, ts + ((uint)(seq - 1) * 480), tag: seq));

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(49, delivered, "49 of 50 packets should be delivered (seq 4 missing).");

        foreach (var p in delivered)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(10000u + ((uint)(p.SequenceNumber - 1) * 480), p.Timestamp,
                $"Timestamp mismatch at seq {p.SequenceNumber}.");
        }

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "Single missing with ts=480, frame=960: gap covered, no lost event.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrementHalfFrameSize_TimerEviction_SingleMissing_NoLostEvent()
    {
        // Timer-based eviction with mismatched timestamp/frame-size and a single gap.
        // Single missing → gap fully covered → no lost event.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 20; i++)
        {
            if (i == 5)
            {
                timestamp += 480;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(19, delivered, "19 of 20 should be delivered.");

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "Single missing with ts=480, frame=960: gap covered even with timer eviction.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrementMinimum120_FrameSize960_TimerEviction_SingleMissing_NoLostEvent()
    {
        // Timer-based eviction with extreme mismatch (120 ts / 960 frame) and a single gap.
        // Gap fully covered → no lost event.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 20; i++)
        {
            if (i == 5)
            {
                timestamp += 120;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 120;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(19, delivered, "19 of 20 should be delivered.");

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            "Single missing with ts=120, frame=960: gap fully covered.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimestampIncrementHalfFrameSize_BurstLoss_PartiallyUncovered()
    {
        // 960-sample frames, 480-sample ts increment. Burst loss: seq 11-20 missing.
        // lostTimestamp = ts_10 + 960, nextTs (seq 21) = ts_10 + 11*480 = ts_10 + 5280.
        // timestampDiff = 5280 - 960 = 4320 → lost event(s) with total 4320 samples.
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        // Burst loss: skip 11-20
        timestamp += 10 * 480;

        for (ushort i = 21; i <= 50; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += 480;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(40, delivered, "40 of 50 packets should be delivered.");

        foreach (var p in delivered)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(10000u + ((uint)(p.SequenceNumber - 1) * 480), p.Timestamp,
                $"Timestamp mismatch at seq {p.SequenceNumber}.");
        }

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsNotEmpty(lost, "Burst loss of 10 × 480 = 4800 exceeds frame 960 → uncovered gap.");
        var totalLostSamples = lost.Sum(l => l.SamplesPerChannel);
        Assert.AreEqual(4320, totalLostSamples,
            "Total lost samples should be (11*480 - 960) = 4320.");
        foreach (var l in lost)
        {
            Assert.AreEqual(DefaultSsrc, l.Ssrc);
            Assert.IsGreaterThan(0, l.SamplesPerChannel,
                "Each lost event's SamplesPerChannel must be positive.");
        }
    }

    [DoNotParallelize]
    [TestMethod]
    [DataRow(480)]      // 2*480=960=frame → timestampDiff=0 → no lost event
    [DataRow(240)]      // 2*240=480<960 → no lost event
    [DataRow(120)]      // 2*120=240<960 → no lost event
    public async Task TimestampIncrementSmallerThanFrameSize_SingleMissing_NoLostEvent(int tsIncrement)
    {
        // Single missing packet: gap always covered by previous 960-sample frame.
        var spp = (uint)tsIncrement;
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i == 5)
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += spp;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(49, delivered);

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            $"Single missing with tsIncrement={tsIncrement} < frameSize=960: gap covered.");
    }

    [DoNotParallelize]
    [TestMethod]
    [DataRow(480)]      // each gap is single → covered
    [DataRow(240)]
    [DataRow(120)]
    public async Task TimestampIncrementSmallerThanFrameSize_ScatteredSingleMissing_NoLostEvents(int tsIncrement)
    {
        // Multiple scattered single-packet gaps with timestamp/frame mismatch.
        // Each individual gap is covered by its preceding frame → no lost events.
        var spp = (uint)tsIncrement;
        HashSet<ushort> skipped = [5, 15, 25, 35];
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (skipped.Contains(i))
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += spp;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(46, delivered, "46 of 50 packets should be delivered.");

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsEmpty(lost,
            $"All gaps are single-packet with tsIncrement={tsIncrement} < frame=960: all covered.");
    }

    [DoNotParallelize]
    [TestMethod]
    [DataRow(480, 3, true)]     // (3+1)*480=1920 > 960 → uncovered
    [DataRow(480, 1, false)]    // (1+1)*480=960 = 960 → covered (timestampDiff=0)
    [DataRow(240, 3, false)]    // (3+1)*240=960 = 960 → covered
    [DataRow(240, 4, true)]     // (4+1)*240=1200 > 960 → uncovered
    [DataRow(120, 7, false)]    // (7+1)*120=960 = 960 → covered
    [DataRow(120, 8, true)]     // (8+1)*120=1080 > 960 → uncovered
    public async Task TimestampIncrementSmallerThanFrameSize_ConsecutiveMissing_CoverageThreshold(
        int tsIncrement, int missCount, bool expectLostEvent)
    {
        // Parametric test: consecutive missing packets near the coverage boundary.
        // Lost event emitted only when (missCount+1)*tsIncrement > frameSize (960).
        var spp = (uint)tsIncrement;
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        ushort missStart = 5;
        ushort missEnd = (ushort)(missStart + missCount - 1);

        for (ushort i = 1; i <= 50; i++)
        {
            if (i >= missStart && i <= missEnd)
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += spp;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(50 - missCount, delivered);

        foreach (var p in delivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);

        var lost = packets.Where(p => p.IsLost).ToList();
        if (expectLostEvent)
        {
            Assert.IsNotEmpty(lost,
                $"{missCount} missing × {tsIncrement} ts: gap exceeds frame → lost event expected.");
            foreach (var l in lost)
            {
                Assert.AreEqual(DefaultSsrc, l.Ssrc);
                Assert.IsGreaterThan(0, l.SamplesPerChannel,
                    "SamplesPerChannel must be positive for uncovered gap.");
            }

            int expectedUncovered = (missCount + 1) * tsIncrement - 960;
            var totalLostSamples = lost.Sum(l => l.SamplesPerChannel);
            Assert.AreEqual(expectedUncovered, totalLostSamples,
                $"Total lost samples should be ({missCount + 1}×{tsIncrement} - 960) = {expectedUncovered}.");
        }
        else
        {
            Assert.IsEmpty(lost,
                $"{missCount} missing × {tsIncrement} ts: gap covered by 960-sample frame → no lost event.");
        }
    }

    [DoNotParallelize]
    [TestMethod]
    [DataRow(480, 10)]      // 11*480 - 960 = 4320 uncovered
    [DataRow(240, 10)]      // 11*240 - 960 = 1680 uncovered
    [DataRow(120, 10)]      // 11*120 - 960 = 360  uncovered
    public async Task TimestampIncrementSmallerThanFrameSize_BurstLoss_ReducedLostSamples(
        int tsIncrement, int burstLen)
    {
        // Burst loss with various ts increments. The lost event samples are reduced
        // because the previous frame already covers part of the gap.
        var spp = (uint)tsIncrement;
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += spp;
        }

        // Burst loss
        timestamp += (uint)(burstLen * (int)spp);

        for (ushort i = (ushort)(11 + burstLen); i <= 50; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += spp;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var delivered = packets.Where(p => !p.IsLost).ToList();
        Assert.HasCount(50 - burstLen, delivered);

        var lost = packets.Where(p => p.IsLost).ToList();
        Assert.IsNotEmpty(lost, "Burst loss should produce lost events.");

        int expectedUncovered = (burstLen + 1) * tsIncrement - 960;
        var totalLostSamples = lost.Sum(l => l.SamplesPerChannel);
        Assert.AreEqual(expectedUncovered, totalLostSamples,
            $"Total lost = ({burstLen + 1}×{tsIncrement} - 960) = {expectedUncovered}.");

        foreach (var l in lost)
        {
            Assert.AreEqual(DefaultSsrc, l.Ssrc);
            Assert.IsGreaterThan(0, l.SamplesPerChannel);
        }
    }

    // ================================================================
    // Ring Buffer Aliasing
    // ================================================================

    [TestMethod]
    public void RingBufferAliasing_CorrectBehavior()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
            ResynchronizationDuration = 1000,
            ResynchronizationThreshold = 6,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;

        handler.Handle(Packet(1, timestamp));
        handler.Handle(Packet(2, timestamp + DefaultFrameSamples));

        // Seq 23 aliases into the ring buffer slot of seq 3's position
        ushort aliasedSeq = 23;
        var aliasedTs = timestamp + ((uint)(aliasedSeq - 1) * DefaultFrameSamples);
        handler.Handle(Packet(aliasedSeq, aliasedTs));

        for (ushort i = 4; i <= 45; i++)
        {
            if (i == aliasedSeq) continue;
            handler.Handle(Packet(i, timestamp + ((uint)(i - 1) * DefaultFrameSamples)));
        }

        var seq3Lost = packets.FirstOrDefault(p => p.IsLost && p.SequenceNumber == 3);
        Assert.IsTrue(seq3Lost.IsLost, "Seq 3 (never sent) should be reported as lost.");
        Assert.AreEqual(DefaultSsrc, seq3Lost.Ssrc);

        var seq23 = packets.FirstOrDefault(p => p.SequenceNumber == 23);
        Assert.IsFalse(seq23.IsLost, "Seq 23 should be recovered successfully.");
        Assert.AreEqual(DefaultSsrc, seq23.Ssrc);
    }

    // ================================================================
    // Timer / Idle Timeout
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerForceEvicts_BufferedPackets()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;
        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 5);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerForceEvicts_WithMissingPacket()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp));
        handler.Handle(Packet(3, timestamp + (2 * DefaultFrameSamples)));
        handler.Handle(Packet(4, timestamp + (3 * DefaultFrameSamples)));

        Assert.IsEmpty(packets, "No packets should be emitted before timer fires.");

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 4, new HashSet<ushort> { 2 });
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerResetOnNewPacket()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp));

        for (ushort i = 2; i <= 5; i++)
        {
            await Task.Delay(50, context.CancellationToken).ConfigureAwait(false);
            timestamp += DefaultFrameSamples;
            handler.Handle(Packet(i, timestamp));
        }

        // Timer not fired yet (last reset ~50ms ago, timer is 200ms)
        Assert.IsEmpty(packets, "Timer should not fire while packets keep arriving.");

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        Assert.HasCount(5, packets, "All 5 packets should be emitted after timer fires.");
        foreach (var p in packets)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.IsFalse(p.IsLost);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task IdleTimeoutRemovesState_NewStreamReInitializes()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 100,
            StartupDuration = 0,
            ResynchronizationDuration = 100,
            IdleTimeout = 200,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp));
        handler.Handle(Packet(2, timestamp + DefaultFrameSamples));

        // Wait for buffer timer + idle timer + margin
        await Task.Delay(600, context.CancellationToken).ConfigureAwait(false);

        int countAfterIdle = packets.Count;
        Assert.IsGreaterThan(0, countAfterIdle, "First stream packets should be emitted.");

        // New stream after state removal → fresh initialization
        handler.Handle(Packet(500, 5000000));
        handler.Handle(Packet(501, 5000000 + DefaultFrameSamples));

        await Task.Delay(400, context.CancellationToken).ConfigureAwait(false);

        var newPackets = packets.Skip(countAfterIdle)
            .Where(p => !p.IsLost && (p.SequenceNumber is 500 or 501))
            .ToList();
        Assert.IsNotEmpty(newPackets, "New packets should be handled after re-initialization.");
        foreach (var p in newPackets)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task SinglePacket_EventuallyEmitted()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
        });

        handler.Handle(Packet(1, 10000, tag: 77));

        await Task.Delay(200, context.CancellationToken).ConfigureAwait(false);

        Assert.IsNotEmpty(packets, "Single packet should eventually be emitted.");
        var p = packets.First(p => !p.IsLost);
        Assert.AreEqual(1, p.SequenceNumber);
        Assert.AreEqual(10000u, p.Timestamp);
        Assert.AreEqual(DefaultSsrc, p.Ssrc);
        Assert.AreEqual(77, p.Tag);
        Assert.IsFalse(p.DecodeFec);
        Assert.AreEqual(0, p.SamplesPerChannel);
        Assert.AreEqual(0, p.FecTag);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_ManyPacketsAcrossGap()
    {
        // Buffer holds packets on both sides of a gap; timer force-evicts everything.
        // BufferDuration=400 → bufferSamples=19200, which exceeds 20×960=18240 total range,
        // so no synchronous eviction is possible.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        // Send 1-10, skip 11, send 12-20
        for (ushort i = 1; i <= 20; i++)
        {
            if (i == 11)
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        Assert.IsEmpty(packets, "Buffer holds everything; no synchronous eviction.");

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 20, new HashSet<ushort> { 11 });

        // Verify tags on delivered packets
        foreach (var p in packets.Where(p => !p.IsLost))
            Assert.AreEqual(p.SequenceNumber, p.Tag, $"Tag mismatch at seq {p.SequenceNumber}.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_MultipleScatteredGaps()
    {
        // Multiple gaps in the buffer, all resolved by timer eviction.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        HashSet<ushort> skipped = [4, 8, 15];

        for (ushort i = 1; i <= 20; i++)
        {
            if (skipped.Contains(i))
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 20, skipped);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_ConsecutiveBurstLoss()
    {
        // Burst loss in the middle: 1-5 arrive, 6-10 missing, 11-15 arrive.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        for (ushort i = 1; i <= 15; i++)
        {
            if (i is >= 6 and <= 10)
            {
                timestamp += DefaultFrameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, DefaultFrameSamples, 15,
            new HashSet<ushort> { 6, 7, 8, 9, 10 });
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_SmallFrameSize_SynchronousEvictionImpossible()
    {
        // 120-sample frames: 30 packets span 3480 samples, far below the 5640
        // threshold for synchronous eviction. Only the timer can drain the buffer.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        const int frameSamples = 120;
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 10)
            {
                timestamp += frameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += frameSamples;
        }

        Assert.IsEmpty(packets, "120-sample packets can't trigger synchronous eviction.");

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, frameSamples, 30, new HashSet<ushort> { 10 });
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_NonStandardFrameSize()
    {
        // 360-sample non-standard frames, startup == buffer → timer only.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        const int frameSamples = 360;
        uint timestamp = 10000;

        for (ushort i = 1; i <= 20; i++)
        {
            handler.Handle(Packet(i, timestamp, frameSamples: frameSamples));
            timestamp += frameSamples;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, frameSamples, 20);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_NonStandardFrameSize_WithGap()
    {
        // 360-sample non-standard frames with a gap, timer evicts.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        const int frameSamples = 360;
        uint timestamp = 10000;

        for (ushort i = 1; i <= 20; i++)
        {
            if (i == 7)
            {
                timestamp += frameSamples;
                continue;
            }
            handler.Handle(Packet(i, timestamp, tag: i, frameSamples: frameSamples));
            timestamp += frameSamples;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, frameSamples, 20, new HashSet<ushort> { 7 });

        var lost = packets.First(p => p.IsLost);
        Assert.IsTrue(lost.DecodeFec, "FEC should reference seq 8.");
        Assert.AreEqual(8, lost.FecTag);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_ThenNewPacketsArriveSynchronously()
    {
        // Timer evicts buffered packets; then enough new packets arrive
        // to trigger synchronous eviction of the new batch.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp, tag: 1));
        handler.Handle(Packet(2, timestamp + DefaultFrameSamples, tag: 2));
        handler.Handle(Packet(3, timestamp + (2 * DefaultFrameSamples), tag: 3));

        Assert.IsEmpty(packets);

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        int countAfterTimer = packets.Count;
        Assert.AreEqual(3, countAfterTimer, "Timer should evict all 3 packets.");
        Assert.IsTrue(packets.All(p => !p.IsLost && p.Ssrc == DefaultSsrc));

        // Now send enough packets for synchronous eviction (state was re-set to Active when the next arrives).
        timestamp = 10000 + (3 * DefaultFrameSamples);
        for (ushort i = 4; i <= 60; i++)
        {
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        // Verify the new batch was delivered (may need timer for tail)
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 60);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_MultipleSSRCs_IndependentTimers()
    {
        // Two SSRCs both stuck in the buffer, timer evicts each independently.
        // Timer callbacks fire on separate ThreadPool threads, so thread-safe
        // List access (via lock in CreateHandler) is essential.
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 400,
            StartupDuration = 400,
            ResynchronizationDuration = 800,
            IdleTimeout = 5000,
        });

        uint ts1 = 10000, ts2 = 50000;
        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, ts1, ssrc: 1000));
            ts1 += DefaultFrameSamples;
        }
        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, ts2, ssrc: 2000));
            ts2 += DefaultFrameSamples;
        }

        Assert.IsEmpty(packets);

        await Task.Delay(800, context.CancellationToken).ConfigureAwait(false);

        var ssrc1 = packets.Where(p => p.Ssrc == 1000 && !p.IsLost).ToList();
        var ssrc2 = packets.Where(p => p.Ssrc == 2000 && !p.IsLost).ToList();

        Assert.HasCount(5, ssrc1, "SSRC 1000: all 5 packets evicted by timer.");
        Assert.HasCount(5, ssrc2, "SSRC 2000: all 5 packets evicted by timer.");

        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual((ushort)(i + 1), ssrc1[i].SequenceNumber);
            Assert.AreEqual((ushort)(i + 1), ssrc2[i].SequenceNumber);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TimerEvicts_GapAtStart_LeadingLossNotReported()
    {
        // Missing packet 1, packets 2-5 arrive. Timer evicts.
        // The handler treats seq 2 as the first packet → seq 1 is a "leading loss"
        // that can't be detected (handler doesn't know seq 1 should have existed).
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        for (ushort i = 2; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // All packets delivered, no loss reported (handler initialized on seq 2)
        Assert.HasCount(4, packets);
        Assert.IsTrue(packets.All(p => !p.IsLost));
        Assert.AreEqual(2, packets[0].SequenceNumber);
        Assert.AreEqual(5, packets[3].SequenceNumber);
    }

    // ================================================================
    // Buffer Size Boundaries
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task ExactBufferSizeBoundary_AllDelivered()
    {
        // Default bufferSize = 240*2/5 = 96
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 97; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 97);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task MinimumBufferSize_AllDelivered()
    {
        // BufferDuration=3 → bufferSize=1
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    [TestMethod]
    public void ZeroStartupDuration_ImmediateEviction()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 240,
            StartupDuration = 0,
        });

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 30);
    }

    // ================================================================
    // FEC (Forward Error Correction) Data
    // ================================================================

    [TestMethod]
    public void FecData_SingleMissing_ContainsNextPacketFrame()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 5) { timestamp += DefaultFrameSamples; continue; }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        var lost5 = packets.First(p => p.IsLost);
        Assert.IsTrue(lost5.DecodeFec, "Single lost packet should have DecodeFec.");
        Assert.AreEqual(6, lost5.FecTag, "FEC data should come from next delivered packet (seq 6).");
        Assert.AreEqual(DefaultSsrc, lost5.Ssrc);
    }

    [TestMethod]
    public void FecData_ConsecutiveMissing_MergedWithFec()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            if (i is >= 5 and <= 7) { timestamp += DefaultFrameSamples; continue; }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        var lostEvents = packets.Where(p => p.IsLost).ToList();
        Assert.HasCount(1, lostEvents, "3 consecutive missing → 1 merged lost event.");
        Assert.IsTrue(lostEvents[0].DecodeFec);
        Assert.AreEqual(8, lostEvents[0].FecTag, "FEC from seq 8.");
        Assert.AreEqual(DefaultSsrc, lostEvents[0].Ssrc);
    }

    [TestMethod]
    public void FecData_MultipleGaps_EachHasFec()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        HashSet<ushort> skipped = [5, 15];

        for (ushort i = 1; i <= 30; i++)
        {
            if (skipped.Contains(i)) { timestamp += DefaultFrameSamples; continue; }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        var lostEvents = packets.Where(p => p.IsLost).ToList();
        Assert.HasCount(2, lostEvents, "Two gaps → 2 lost events.");

        Assert.IsTrue(lostEvents[0].DecodeFec);
        Assert.AreEqual(6, lostEvents[0].FecTag, "First gap FEC from seq 6.");

        Assert.IsTrue(lostEvents[1].DecodeFec);
        Assert.AreEqual(16, lostEvents[1].FecTag, "Second gap FEC from seq 16.");

        foreach (var l in lostEvents)
            Assert.AreEqual(DefaultSsrc, l.Ssrc);
    }

    [TestMethod]
    public void FecData_LargeBurst_OnlyLastMergedEventHasFec()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 40; i++)
        {
            if (i is >= 11 and <= 20) { timestamp += DefaultFrameSamples; continue; }
            handler.Handle(Packet(i, timestamp, tag: i));
            timestamp += DefaultFrameSamples;
        }

        var lostEvents = packets.Where(p => p.IsLost).ToList();
        Assert.IsGreaterThan(1, lostEvents.Count, "Large burst should produce multiple merged events.");

        for (int i = 0; i < lostEvents.Count - 1; i++)
            Assert.IsFalse(lostEvents[i].DecodeFec, $"Non-last merged event {i} should not have DecodeFec.");

        Assert.IsTrue(lostEvents[^1].DecodeFec, "Last merged event should have DecodeFec.");
        Assert.AreEqual(21, lostEvents[^1].FecTag, "FEC from seq 21.");

        foreach (var l in lostEvents)
            Assert.AreEqual(DefaultSsrc, l.Ssrc);
    }

    [TestMethod]
    public void FecData_NotPresentOnDeliveredPackets()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        foreach (var p in packets)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.IsFalse(p.IsLost);
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
            Assert.AreEqual(0, p.FecTag);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task FecData_OnForceEvict_ContainsNextStoredFrame()
    {
        var (handler, packets) = CreateHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;
        handler.Handle(Packet(1, timestamp, tag: 1));
        handler.Handle(Packet(3, timestamp + (2 * DefaultFrameSamples), tag: 3));
        handler.Handle(Packet(4, timestamp + (3 * DefaultFrameSamples), tag: 4));

        Assert.IsEmpty(packets);

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        var lost2 = packets.First(p => p.IsLost);
        Assert.IsTrue(lost2.DecodeFec, "Force-evicted lost packet should have DecodeFec.");
        Assert.AreEqual(3, lost2.FecTag, "FEC from stored seq 3.");
        Assert.AreEqual(DefaultSsrc, lost2.Ssrc);
    }

    [TestMethod]
    [DataRow(360)]      // non-standard
    [DataRow(1080)]     // non-standard
    [DataRow(2400)]     // non-standard
    public void FecData_NonStandardFrameSize_SingleMissing(int frameSamples)
    {
        var spp = (uint)frameSamples;
        int packetCount = GetMinPacketCount(frameSamples);
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= packetCount; i++)
        {
            if (i == 5) { timestamp += spp; continue; }
            handler.Handle(Packet(i, timestamp, tag: i, frameSamples: frameSamples));
            timestamp += spp;
        }

        var lostEvents = packets.Where(p => p.IsLost).ToList();
        Assert.HasCount(1, lostEvents, "Single missing → 1 lost event.");
        Assert.IsTrue(lostEvents[0].DecodeFec);
        Assert.AreEqual(6, lostEvents[0].FecTag, "FEC from seq 6.");
        Assert.AreEqual(DefaultSsrc, lostEvents[0].Ssrc);
        Assert.AreEqual((int)spp, lostEvents[0].SamplesPerChannel,
            $"Lost event SamplesPerChannel should equal {spp}.");
    }

    // ================================================================
    // Variable Frame Sizes Mid-Stream
    // ================================================================

    [TestMethod]
    public void FrameSizeChange_MidStream_AllDelivered()
    {
        // 10 packets at 20ms, 10 at 10ms, 10 at 40ms
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        uint[] expectedTs = new uint[30];
        uint[] sizes = [960, 480, 1920];

        for (int i = 0; i < 30; i++)
        {
            expectedTs[i] = timestamp;
            ushort seq = (ushort)(i + 1);
            int fs = (int)sizes[i / 10];
            handler.Handle(Packet(seq, timestamp, frameSamples: fs));
            timestamp += sizes[i / 10];
        }

        Assert.HasCount(30, packets);
        for (int i = 0; i < 30; i++)
        {
            var p = packets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(expectedTs[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
            Assert.AreEqual(0, p.FecTag);
        }
    }

    [TestMethod]
    public void FrameSizeChange_IncludingNonStandard_AllDelivered()
    {
        // Cycle through: 360 (non-std), 600 (non-std), 1080 (non-std), 960 (standard)
        uint[] frameSizes = [360, 600, 1080, 960];

        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        uint[] expectedTs = new uint[30];

        for (ushort i = 1; i <= 30; i++)
        {
            expectedTs[i - 1] = timestamp;
            var fs = (int)frameSizes[(i - 1) % frameSizes.Length];
            handler.Handle(Packet(i, timestamp, frameSamples: fs));
            timestamp += frameSizes[(i - 1) % frameSizes.Length];
        }

        Assert.HasCount(30, packets);
        for (int i = 0; i < 30; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(expectedTs[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
            Assert.IsFalse(p.DecodeFec);
        }
    }

    [TestMethod]
    public void FrameSizeChange_WithReordering()
    {
        uint[] frameSizes = [960, 480, 240, 120, 1920, 2880]; // cycling
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += frameSizes[i % frameSizes.Length];
        }

        // Send: 1, 3, 2, then 4-30
        handler.Handle(Packet(1, timestamps[0], frameSamples: (int)frameSizes[0]));
        handler.Handle(Packet(3, timestamps[2], frameSamples: (int)frameSizes[2]));
        handler.Handle(Packet(2, timestamps[1], frameSamples: (int)frameSizes[1]));

        for (ushort i = 4; i <= 30; i++)
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: (int)frameSizes[(i - 1) % frameSizes.Length]));

        Assert.HasCount(30, packets);
        for (int i = 0; i < 30; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(timestamps[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task FrameSizeChange_WithMissingPackets()
    {
        // 10 packets at 20ms, then 20 at 10ms. Missing: seq 5 and seq 15.
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += i < 10 ? 960u : 480u;
        }

        for (ushort i = 1; i <= 30; i++)
        {
            if (i is 5 or 15) continue;
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: i <= 10 ? 960 : 480));
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        Assert.HasCount(30, packets, "28 delivered + 2 lost = 30 entries.");
        for (int i = 0; i < 30; i++)
        {
            var p = packets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(timestamps[i], p.Timestamp);

            if (expectedSeq is 5 or 15)
            {
                Assert.IsTrue(p.IsLost);
                Assert.IsTrue(p.DecodeFec, $"Lost packet {expectedSeq} should have DecodeFec.");
            }
            else
            {
                Assert.IsFalse(p.IsLost);
                Assert.IsFalse(p.DecodeFec);
                Assert.AreEqual(0, p.SamplesPerChannel);
                Assert.AreEqual(0, p.FecTag);
            }
        }
    }

    [TestMethod]
    public void AllStandardFrameSizes_Sequential()
    {
        // 5 packets per standard frame size, changing immediately between sizes.
        uint[] frameSizes = [120, 240, 480, 960, 1920, 2880, 5760];
        int packetsPerSize = 5;
        int totalPackets = frameSizes.Length * packetsPerSize;

        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        uint[] expectedTs = new uint[totalPackets];
        int pktIdx = 0;

        foreach (var fs in frameSizes)
        {
            for (int j = 0; j < packetsPerSize; j++)
            {
                expectedTs[pktIdx] = timestamp;
                handler.Handle(Packet((ushort)(pktIdx + 1), timestamp, frameSamples: (int)fs));
                timestamp += fs;
                pktIdx++;
            }
        }

        Assert.HasCount(totalPackets, packets);
        for (int i = 0; i < totalPackets; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(expectedTs[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
        }
    }

    [TestMethod]
    public void NonStandardFrameSizes_Sequential()
    {
        // 5 packets per non-standard frame size (but valid multiples of 120).
        uint[] frameSizes = [360, 600, 720, 1080, 1440, 2400, 3600];
        int packetsPerSize = 5;
        int totalPackets = frameSizes.Length * packetsPerSize;

        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;
        uint[] expectedTs = new uint[totalPackets];
        int pktIdx = 0;

        foreach (var fs in frameSizes)
        {
            for (int j = 0; j < packetsPerSize; j++)
            {
                expectedTs[pktIdx] = timestamp;
                handler.Handle(Packet((ushort)(pktIdx + 1), timestamp, frameSamples: (int)fs));
                timestamp += fs;
                pktIdx++;
            }
        }

        Assert.HasCount(totalPackets, packets);
        for (int i = 0; i < totalPackets; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(expectedTs[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task BurstLossAtFrameSizeTransition()
    {
        // Packets 1-10: 20ms (960), skip 11-15, packets 16-30: 10ms (480)
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += i < 10 ? 960u : 480u;
        }

        for (ushort i = 1; i <= 10; i++)
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: 960));

        for (ushort i = 16; i <= 30; i++)
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: 480));

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // 5 lost packets (11-15) merged: total lost = timestamps[15] - (timestamps[9] + 960) = 2400
        Assert.HasCount(26, packets, "25 delivered + 1 merged lost = 26 entries.");

        for (int i = 0; i < 10; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(i + 1), p.SequenceNumber);
            Assert.AreEqual(timestamps[i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }

        {
            var p = packets[10];
            Assert.IsTrue(p.IsLost);
            Assert.AreEqual(timestamps[10], p.Timestamp);
            Assert.AreEqual(2400, p.SamplesPerChannel);
            Assert.IsTrue(p.DecodeFec);
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
        }

        for (int i = 0; i < 15; i++)
        {
            var p = packets[11 + i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(16 + i), p.SequenceNumber);
            Assert.AreEqual(timestamps[15 + i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task LargeToSmallFrameTransition_WithBurstLoss()
    {
        // 60ms (2880) → burst loss → 2.5ms (120)
        var (handler, packets) = CreateHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[40];
        for (int i = 0; i < 40; i++)
        {
            timestamps[i] = timestamp;
            timestamp += (uint)(i < 10 ? 2880 : 120);
        }

        for (ushort i = 1; i <= 10; i++)
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: 2880));

        // Skip 11-15

        for (ushort i = 16; i <= 40; i++)
            handler.Handle(Packet(i, timestamps[i - 1], frameSamples: 120));

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // 5 lost packets merged: total lost = timestamps[15] - (timestamps[9] + 2880) = 600
        Assert.HasCount(36, packets, "35 delivered + 1 merged lost = 36 entries.");

        for (int i = 0; i < 10; i++)
        {
            var p = packets[i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.IsFalse(p.IsLost);
            Assert.AreEqual(timestamps[i], p.Timestamp);
        }

        {
            var p = packets[10];
            Assert.IsTrue(p.IsLost);
            Assert.AreEqual(timestamps[10], p.Timestamp);
            Assert.AreEqual(600, p.SamplesPerChannel);
            Assert.IsTrue(p.DecodeFec);
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
        }

        for (int i = 0; i < 25; i++)
        {
            var p = packets[11 + i];
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.AreEqual((ushort)(16 + i), p.SequenceNumber);
            Assert.AreEqual(timestamps[15 + i], p.Timestamp);
            Assert.IsFalse(p.IsLost);
        }
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task SmallestFrameSize_WithMissingPackets()
    {
        // 2.5ms (120 samples) with scattered losses
        const uint spp = 120;

        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 50; i++)
        {
            if (i is 10 or 25 or 40)
            {
                timestamp += spp;
                continue;
            }
            handler.Handle(Packet(i, timestamp, frameSamples: (int)spp));
            timestamp += spp;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertSequenceWithLoss(packets, 1, 10000, spp, 50, new HashSet<ushort> { 10, 25, 40 });
    }

    [TestMethod]
    public void LargestFrameSize_WithReordering()
    {
        // 120ms (5760 samples) with reordering
        const uint spp = 5760;
        var (handler, packets) = CreateHandler();
        uint ts = 10000;

        handler.Handle(Packet(1, ts, frameSamples: (int)spp));
        handler.Handle(Packet(3, ts + (2 * spp), frameSamples: (int)spp));
        handler.Handle(Packet(2, ts + spp, frameSamples: (int)spp));

        for (ushort i = 4; i <= 30; i++)
            handler.Handle(Packet(i, ts + ((uint)(i - 1) * spp), frameSamples: (int)spp));

        AssertOrderedDelivery(packets, 1, 10000, spp, 30);
    }

    // ================================================================
    // Stress / Large Volume
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task LargePacketVolume_10000Packets()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10000; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertOrderedDelivery(packets, 1, 10000, DefaultFrameSamples, 10000);
    }

    [TestMethod]
    public void FullSequenceNumberCycle_65536Packets()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 0;

        for (int i = 0; i < 65536; i++)
        {
            handler.Handle(Packet((ushort)i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        Assert.HasCount(65536, packets);

        ushort last = 0;
        bool first = true;
        foreach (var p in packets)
        {
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
            Assert.IsFalse(p.IsLost, $"Packet {p.SequenceNumber} should not be lost.");
            Assert.IsFalse(p.DecodeFec);
            Assert.AreEqual(0, p.SamplesPerChannel);
            Assert.AreEqual(0, p.FecTag);
            if (!first)
            {
                ushort diff = (ushort)(p.SequenceNumber - last);
                Assert.IsGreaterThan((ushort)0, diff, $"Non-monotonic: {last} → {p.SequenceNumber}.");
                Assert.IsLessThan((ushort)32768, diff, $"Wrapped incorrectly: {last} → {p.SequenceNumber}.");
            }
            last = p.SequenceNumber;
            first = false;
        }
    }

    // ================================================================
    // Edge: Scattered outliers / anomalous input
    // ================================================================

    [TestMethod]
    public void ScatteredOutliers_NormalFlowContinues()
    {
        var (handler, packets) = CreateHandler(new() { ResynchronizationThreshold = 3 });
        uint timestamp = 10000;

        handler.Handle(Packet(1, timestamp));

        // Widely separated outliers that never form a contiguous stream
        handler.Handle(Packet(20000, 50000000));
        handler.Handle(Packet(40000, 90000000));
        handler.Handle(Packet(60000, 130000000));

        for (ushort i = 2; i <= 30; i++)
        {
            timestamp += DefaultFrameSamples;
            handler.Handle(Packet(i, timestamp));
        }

        var normalDelivered = packets.Where(p => !p.IsLost && p.SequenceNumber is >= 1 and <= 30).ToList();
        Assert.IsNotEmpty(normalDelivered, "Normal packets should be delivered despite scattered outliers.");
        foreach (var p in normalDelivered)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
    }

    [TestMethod]
    public void SameSequenceDifferentTimestamp_DoesNotCrash()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Replay seq 5 with wildly different timestamp
        handler.Handle(Packet(5, timestamp + 50000));

        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        Assert.IsNotEmpty(packets);
        foreach (var p in packets)
            Assert.AreEqual(DefaultSsrc, p.Ssrc);
    }

    [TestMethod]
    public void TimestampGoesBackward_DoesNotCrash()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Seq 6 with backward timestamp
        handler.Handle(Packet(6, 10000));

        for (ushort i = 7; i <= 30; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        Assert.IsNotEmpty(packets);
    }

    [TestMethod]
    public void SameTimestampDifferentSequences_DoesNotCrash()
    {
        var (handler, packets) = CreateHandler();
        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(Packet(i, timestamp));
            timestamp += DefaultFrameSamples;
        }

        // Multiple packets sharing the same timestamp
        for (ushort i = 6; i <= 10; i++)
            handler.Handle(Packet(i, timestamp));

        for (ushort i = 11; i <= 30; i++)
        {
            timestamp += DefaultFrameSamples;
            handler.Handle(Packet(i, timestamp));
        }

        Assert.IsNotEmpty(packets);
    }
}
