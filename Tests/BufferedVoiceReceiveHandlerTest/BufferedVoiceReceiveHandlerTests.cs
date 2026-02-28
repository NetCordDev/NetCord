using System.Buffers.Binary;
using System.Diagnostics;

using NetCord.Gateway.Voice;

namespace BufferedVoiceReceiveHandlerTest;

[TestClass]
public sealed class BufferedVoiceReceiveHandlerTests(TestContext context)
{
    private const uint DefaultSsrc = 1234;
    private const uint SamplesPerPacket = 960;
    private const int DefaultPayloadType = 0x78;

    private record struct ReceivedPacket(uint Ssrc, ushort SequenceNumber, uint Timestamp, bool Missed, int Tag, int SamplesPerChannel, bool DecodeFec, int FecTag);

    private static (BufferedVoiceReceiveHandler Handler, List<ReceivedPacket> ReceivedPackets) InitializeHandler(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        BufferedVoiceReceiveHandler handler = new(configuration);

        List<ReceivedPacket> receivedPackets = [];

        handler.VoiceReceive += data =>
        {
            bool isLost = data.IsLost;
            bool decodeFec = false;
            int fecTag = 0;
            int samplesPerChannel = 0;
            if (isLost)
            {
                var lost = data.AsLost();
                samplesPerChannel = lost.SamplesPerChannel;
                decodeFec = lost.DecodeFec;
                if (decodeFec)
                    fecTag = BinaryPrimitives.ReadInt32LittleEndian(lost.FecData[2..]);
            }
            receivedPackets.Add(new ReceivedPacket(
                data.Ssrc,
                data.SequenceNumber,
                data.Timestamp,
                isLost,
                isLost ? 0 : BinaryPrimitives.ReadInt32LittleEndian(data.Frame[2..]),
                samplesPerChannel,
                decodeFec,
                fecTag));
        };

        return (handler, receivedPackets);
    }

    private static VoiceReceiveContext CreateContext(ushort seq, uint timestamp, uint ssrc = DefaultSsrc, int payloadType = DefaultPayloadType, int tag = 0, int frameSamples = (int)SamplesPerPacket)
    {
        var toc = GenerateOpusToc(frameSamples);
        if (toc.Length is 1)
            toc = [toc[0], 0];

        var tagBytes = (stackalloc byte[4]);
        BinaryPrimitives.WriteInt32LittleEndian(tagBytes, tag);

        byte[] frame = [.. toc, .. tagBytes];
        return new(new(ssrc, timestamp, payloadType, seq, tag), frame);
    }

    public static byte[] GenerateOpusToc(int targetSamplesPerChannel, int Fs = 48000)
    {
        // Define frame sizes and their corresponding base TOC bytes that map to your code's branches.
        // Format: (SamplesPerFrame, BaseTocByte)
        var validConfigurations = new (int Samples, byte BaseToc)[]
        {
            (Fs * 60 / 1000, 0x18),  // 2880 samples (SILK) -> hits `audiosize == 3`
            ((Fs << 2) / 100, 0x10), // 1920 samples (SILK) -> hits `else audiosize`
            ((Fs << 1) / 100, 0x08), // 960  samples (SILK) -> hits `else audiosize`
            ((Fs << 0) / 100, 0x00), // 480  samples (SILK) -> hits `else audiosize`
            ((Fs << 1) / 400, 0x88), // 240  samples (CELT) -> hits `(data[0] & 0x80) is not 0`
            ((Fs << 0) / 400, 0x80)  // 120  samples (CELT) -> hits `(data[0] & 0x80) is not 0`
        };

        foreach (var config in validConfigurations)
        {
            // Find a frame size that perfectly divides the requested total samples
            if (targetSamplesPerChannel % config.Samples == 0)
            {
                int numberOfFrames = targetSamplesPerChannel / config.Samples;

                if (numberOfFrames > 63)
                    continue;

                if (numberOfFrames == 1)
                {
                    return [(byte)(config.BaseToc | 0x00)];
                }
                if (numberOfFrames == 2)
                {
                    return [(byte)(config.BaseToc | 0x01)];
                }
                if (numberOfFrames >= 3)
                {
                    return [(byte)(config.BaseToc | 0x03), (byte)numberOfFrames];
                }
            }
        }

        throw new ArgumentException($"Cannot generate a valid Opus TOC for {targetSamplesPerChannel} samples at Fs={Fs}. The value must be divisible by standard frame sizes and require <= 63 frames.");
    }

    /// <summary>
    /// Asserts that all non-missed packets in the list are in strictly increasing sequence order (using ushort wraparound arithmetic).
    /// </summary>
    private static void AssertMonotonicDelivery(List<ReceivedPacket> receivedPackets)
    {
        ushort last = 0;
        bool first = true;
        foreach (var p in receivedPackets)
        {
            if (!p.Missed)
            {
                if (!first)
                {
                    short diff = (short)(p.SequenceNumber - last);
                    Assert.IsGreaterThan((short)0, diff, $"Non-monotonic delivery: {last} -> {p.SequenceNumber}");
                }
                last = p.SequenceNumber;
                first = false;
            }
        }
    }

    /// <summary>
    /// Asserts that the received packets exactly match a contiguous sequence with no misses,
    /// including correct timestamps.
    /// </summary>
    private static void AssertExactOrderedDelivery(List<ReceivedPacket> receivedPackets, ushort startSeq, uint startTimestamp, uint samplesPerPacket, int count)
    {
        Assert.HasCount(count, receivedPackets, $"Expected {count} packets but got {receivedPackets.Count}.");
        for (int i = 0; i < count; i++)
        {
            var expected = (ushort)(startSeq + i);
            var expectedTimestamp = startTimestamp + ((uint)i * samplesPerPacket);
            var p = receivedPackets[i];
            Assert.IsFalse(p.Missed, $"Packet {expected} was marked as missed.");
            Assert.IsFalse(p.DecodeFec, $"Delivered packet {expected} should not have DecodeFec set.");
            Assert.AreEqual(expected, p.SequenceNumber, $"Expected seq {expected} at position {i}, got {p.SequenceNumber}.");
            Assert.AreEqual(expectedTimestamp, p.Timestamp, $"Expected timestamp {expectedTimestamp} at position {i}, got {p.Timestamp}.");
        }
    }

    /// <summary>
    /// Asserts that the received packets exactly match a contiguous sequence, with specified sequences
    /// marked as missed (lost). Consecutive lost packets are merged into larger lost events (up to
    /// MaxSamplesPerPacket = 5760 samples each). Verifies timestamps and SamplesPerChannel for merged
    /// lost events. Losses at the very start or end of the range are not expected to appear as lost
    /// events, because the handler has no context to know they were missed.
    /// </summary>
    private static void AssertExactSequenceWithLoss(List<ReceivedPacket> receivedPackets, ushort startSeq, uint startTimestamp, uint samplesPerPacket, int totalCount, HashSet<ushort> missedSequences)
    {
        const int MaxSamplesPerLostPacket = 120 * (48_000 / 1000); // 5760

        // Boundary losses (at the start or end of the sequence range) are not emitted as lost
        // events because the handler has no prior/subsequent context to detect them.

        // Skip leading missed sequences.
        int leadingSkip = 0;
        while (leadingSkip < totalCount && missedSequences.Contains((ushort)(startSeq + leadingSkip)))
            leadingSkip++;

        // Skip trailing missed sequences.
        int trailingSkip = 0;
        while (trailingSkip < totalCount - leadingSkip && missedSequences.Contains((ushort)(startSeq + totalCount - 1 - trailingSkip)))
            trailingSkip++;

        int adjustedEnd = totalCount - trailingSkip;

        int receivedIdx = 0;
        int i = leadingSkip;
        while (i < adjustedEnd)
        {
            var expectedSeq = (ushort)(startSeq + i);
            var expectedTimestamp = startTimestamp + ((uint)i * samplesPerPacket);

            if (!missedSequences.Contains(expectedSeq))
            {
                // Delivered packet
                Assert.IsTrue(receivedIdx < receivedPackets.Count,
                    $"Ran out of received packets at index {receivedIdx}; expected delivered seq {expectedSeq}.");
                var p = receivedPackets[receivedIdx++];
                Assert.AreEqual(expectedSeq, p.SequenceNumber,
                    $"Expected seq {expectedSeq} at index {receivedIdx - 1}, got {p.SequenceNumber}.");
                Assert.AreEqual(expectedTimestamp, p.Timestamp,
                    $"Expected timestamp {expectedTimestamp} at index {receivedIdx - 1}, got {p.Timestamp}.");
                Assert.IsFalse(p.Missed,
                    $"Packet {expectedSeq} was marked as missed but should be delivered.");
                Assert.IsFalse(p.DecodeFec,
                    $"Delivered packet {expectedSeq} should not have DecodeFec set.");
                i++;
            }
            else
            {
                // Find end of consecutive loss range
                int lossStart = i;
                while (i < adjustedEnd && missedSequences.Contains((ushort)(startSeq + i)))
                    i++;

                int lossCount = i - lossStart;
                uint totalLostSamples = (uint)lossCount * samplesPerPacket;
                uint lossTimestampStart = startTimestamp + ((uint)lossStart * samplesPerPacket);

                // The handler merges consecutive lost packets into events of up to MaxSamplesPerLostPacket.
                // First event: remainder (totalLostSamples % MaxSamplesPerLostPacket), if non-zero.
                // Subsequent events: full MaxSamplesPerLostPacket each.
                uint firstEventSamples = totalLostSamples % (uint)MaxSamplesPerLostPacket;
                int expectedMergedEvents;
                if (firstEventSamples == 0)
                    expectedMergedEvents = (int)(totalLostSamples / (uint)MaxSamplesPerLostPacket);
                else
                    expectedMergedEvents = 1 + (int)((totalLostSamples - firstEventSamples) / (uint)MaxSamplesPerLostPacket);

                uint currentTs = lossTimestampStart;
                for (int e = 0; e < expectedMergedEvents; e++)
                {
                    Assert.IsTrue(receivedIdx < receivedPackets.Count,
                        $"Ran out of received packets at index {receivedIdx}; expected merged lost event for gap at seq {(ushort)(startSeq + lossStart)}.");
                    var p = receivedPackets[receivedIdx++];
                    Assert.IsTrue(p.Missed,
                        $"Expected a lost event at index {receivedIdx - 1}, got delivered packet seq {p.SequenceNumber}.");
                    Assert.AreEqual(currentTs, p.Timestamp,
                        $"Lost event timestamp mismatch at index {receivedIdx - 1}. Expected {currentTs}, got {p.Timestamp}.");

                    uint expectedSamples;
                    if (e == 0 && firstEventSamples != 0)
                        expectedSamples = firstEventSamples;
                    else
                        expectedSamples = (uint)MaxSamplesPerLostPacket;

                    Assert.AreEqual((int)expectedSamples, p.SamplesPerChannel,
                        $"Lost event SamplesPerChannel mismatch at index {receivedIdx - 1}. Expected {expectedSamples}, got {p.SamplesPerChannel}.");

                    bool isLastMergedEvent = (e == expectedMergedEvents - 1);
                    Assert.AreEqual(isLastMergedEvent, p.DecodeFec,
                        $"Lost event at index {receivedIdx - 1}: DecodeFec should be {isLastMergedEvent}.");

                    currentTs += expectedSamples;
                }
            }
        }

        Assert.AreEqual(receivedPackets.Count, receivedIdx,
            $"Expected {receivedIdx} total entries but got {receivedPackets.Count}.");
    }

    // ================================================================
    // Basic Ordered / Reordered / Missing
    // ================================================================

    [TestMethod]
    public void TestOrderedBuffer()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestReorderedBuffer()
    {
        var (handler, receivedPackets) = InitializeHandler();

        List<ushort> sendOrder = [1, 3, 2, 4, 6, 5, 7, 8, 9, 10, 12, 11];

        for (ushort i = 13; i <= 30; i++)
            sendOrder.Add(i);

        uint timestampBase = 10000;

        int sendCount = sendOrder.Count;

        for (var i = 0; i < sendCount; i++)
        {
            var seq = sendOrder[i];
            var timestamp = timestampBase + ((uint)(seq - 1) * SamplesPerPacket);
            handler.Handle(CreateContext(seq, timestamp));
        }

        // All packets should be reordered and delivered exactly in order
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestMissingPacket()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 3)
            {
                timestamp += SamplesPerPacket;
                continue;
            }

            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Single lost packet appears as a Lost event with SamplesPerChannel = 960
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 30, new HashSet<ushort> { 3 });
    }

    [TestMethod]
    public void TestRingBufferAliasing()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
            ResynchronizationDuration = 1000,
            ResynchronizationThreshold = 6,
            IdleTimeout = 1000
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(2, timestamp + SamplesPerPacket));

        ushort aliasedSeq = 23;
        var aliasedTimestamp = timestamp + ((uint)(aliasedSeq - 1) * SamplesPerPacket);
        handler.Handle(CreateContext(aliasedSeq, aliasedTimestamp));

        for (ushort i = 4; i <= 45; i++)
        {
            if (i == aliasedSeq) continue;

            var t = timestamp + ((uint)(i - 1) * SamplesPerPacket);
            handler.Handle(CreateContext(i, t));
        }

        // Seq 3 was never sent. It should appear as a Lost event.
        var seq3Lost = receivedPackets.FirstOrDefault(p => p.Missed && p.SequenceNumber == 3);
        Assert.IsTrue(seq3Lost.Missed, "Sequence 3 should appear as a missed event.");

        var seq23 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 23);
        Assert.IsFalse(seq23.Missed, "Sequence 23 should have been recovered successfully.");
    }

    [TestMethod]
    public void TestResynchronizationThreshold()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(2, timestamp + SamplesPerPacket));

        ushort jumpBase = 502;

        uint jumpTimestamp = timestamp + ((uint)(jumpBase - 1) * SamplesPerPacket);

        handler.Handle(CreateContext(jumpBase, jumpTimestamp));
        handler.Handle(CreateContext((ushort)(jumpBase + 1), jumpTimestamp + SamplesPerPacket));
        handler.Handle(CreateContext((ushort)(jumpBase + 2), jumpTimestamp + (2 * SamplesPerPacket)));

        for (ushort i = 3; i <= 25; i++)
        {
            uint t = jumpTimestamp + (i * SamplesPerPacket);
            handler.Handle(CreateContext((ushort)(jumpBase + i), t));
        }

        // Sequences 1 and 2 should have been force-evicted during resync reset
        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync reset.");

        var seq2 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 2);
        Assert.IsFalse(seq2.Missed, "Sequence 2 should have been force-evicted during resync reset.");

        var thresholdSeq = receivedPackets.FirstOrDefault(p => p.SequenceNumber == jumpBase + 2);
        Assert.IsFalse(thresholdSeq.Missed, "The sequence that triggered the resync should be handled successfully as the new baseline.");

        // New range packets should all be delivered
        var newRangeDelivered = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpBase).ToList();
        Assert.IsNotEmpty(newRangeDelivered, "New range packets should be delivered after resync.");
    }

    // ================================================================
    // Sequence Number Overflow / Wraparound (ushort boundary)
    // ================================================================

    [TestMethod]
    public void TestSequenceNumberWraparound()
    {
        var (handler, receivedPackets) = InitializeHandler();

        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            handler.Handle(CreateContext(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Exact delivery: 32 packets from 65530 through wraparound
        AssertExactOrderedDelivery(receivedPackets, startSeq, 10000, SamplesPerPacket, 32);
    }

    [TestMethod]
    public void TestSequenceNumberWraparoundWithReordering()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Out of order near wraparound: 65534, 0, 65535, 1, 2, 3, ...
        List<ushort> sendOrder = [65534, 0, 65535, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];

        foreach (var seq in sendOrder)
        {
            var seqOffset = (ushort)(seq - 65534);
            var timestamp = timestampBase + (seqOffset * SamplesPerPacket);
            handler.Handle(CreateContext(seq, timestamp));
        }

        // Should deliver all 28 packets in order: 65534, 65535, 0, 1, ..., 25
        AssertExactOrderedDelivery(receivedPackets, 65534, 10000, SamplesPerPacket, 28);
    }

    [TestMethod]
    public void TestSequenceNumberWraparoundMissingPacket()
    {
        var (handler, receivedPackets) = InitializeHandler();

        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            if (seq == 65535)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Single missing packet (65535) appears as a Lost event
        AssertExactSequenceWithLoss(receivedPackets, startSeq, 10000, SamplesPerPacket, 32, new HashSet<ushort> { 65535 });
    }

    [TestMethod]
    public void TestSequenceNumberWraparoundMissingAtZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        ushort startSeq = 65530;
        uint timestamp = 10000;

        for (int i = 0; i < 32; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            if (seq == 0)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Single missing packet (seq 0) appears as a Lost event
        AssertExactSequenceWithLoss(receivedPackets, startSeq, 10000, SamplesPerPacket, 32, new HashSet<ushort> { 0 });
    }

    // ================================================================
    // Timestamp Overflow / Wraparound (uint boundary)
    // ================================================================

    [TestMethod]
    public void TestTimestampWraparound()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = uint.MaxValue - (10 * SamplesPerPacket);

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, uint.MaxValue - (10 * SamplesPerPacket), SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestTimestampStartingAtZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 0;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 0, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestTimestampExactlyAtMaxValue()
    {
        var (handler, receivedPackets) = InitializeHandler();

        // Start so that one packet lands exactly at uint.MaxValue
        uint timestamp = uint.MaxValue - (5 * SamplesPerPacket);

        for (ushort i = 1; i <= 20; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, uint.MaxValue - (5 * SamplesPerPacket), SamplesPerPacket, 20);
    }

    [TestMethod]
    public void TestBothSequenceAndTimestampWraparound()
    {
        var (handler, receivedPackets) = InitializeHandler();

        ushort startSeq = 65530;
        uint timestamp = uint.MaxValue - (5 * SamplesPerPacket);

        for (int i = 0; i < 30; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            handler.Handle(CreateContext(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        AssertExactOrderedDelivery(receivedPackets, startSeq, uint.MaxValue - (5 * SamplesPerPacket), SamplesPerPacket, 30);
    }

    // ================================================================
    // Duplicate Packets
    // ================================================================

    [TestMethod]
    public void TestDuplicatePacketIgnored()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));

            if (i % 5 == 0)
                handler.Handle(CreateContext(i, timestamp));

            timestamp += SamplesPerPacket;
        }

        // All 30 packets should be delivered exactly once, in order
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestConsecutiveDuplicates()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            handler.Handle(CreateContext(i, timestamp));
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // All 30 packets should be delivered exactly once, in order
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestDuplicateOfLatestPacket()
    {
        // The handler has a special `sequenceNumberDiff is 0` check for duplicates of the latest packet
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(1, timestamp)); // Exact duplicate of "latest"

        timestamp += SamplesPerPacket;
        for (ushort i = 2; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // All 30 should be delivered exactly once
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestDuplicatePacketDoesNotOverwriteOriginalData()
    {
        // Verifies that when a duplicate packet with different data arrives after
        // several intervening packets, the original data is preserved in the buffer.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Send packets 1-30 with tag = sequence number.
        // After every 5-packet batch, re-send a duplicate of the first packet
        // in the batch with a different tag, e.g.:
        //   1,2,3,4,5 → dup(1), 6,7,8,9,10 → dup(6), ...
        for (ushort batch = 0; batch < 6; batch++)
        {
            ushort batchStart = (ushort)(batch * 5 + 1);

            for (ushort j = 0; j < 5; j++)
            {
                ushort seq = (ushort)(batchStart + j);
                handler.Handle(CreateContext(seq, timestamp, tag: seq));
                timestamp += SamplesPerPacket;
            }

            // Duplicate of batchStart arrives after 4 intervening packets
            handler.Handle(CreateContext(batchStart, timestamp - (5 * SamplesPerPacket), tag: batchStart + 10000));
        }

        // Verify that delivered packets have the ORIGINAL tag, not the duplicate's
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.HasCount(30, nonMissed, "All 30 packets should be delivered.");
        foreach (var p in nonMissed)
        {
            Assert.AreEqual(p.SequenceNumber, p.Tag,
                $"Packet {p.SequenceNumber} has tag {p.Tag} but expected {p.SequenceNumber}. Duplicate may have overwritten original.");
        }
    }

    [TestMethod]
    public void TestDelayedDuplicateAfterEvictionPreservesOriginalData()
    {
        // After the original packet has been evicted from the buffer,
        // a delayed duplicate with different data should be ignored entirely.
        // With default config, packets start getting evicted around seq 7.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Send packets 1-10 (by seq 7, packet 1 is evicted)
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Delayed duplicates of early packets with different tags
        handler.Handle(CreateContext(1, 10000, tag: 9001));
        handler.Handle(CreateContext(2, 10000 + SamplesPerPacket, tag: 9002));
        handler.Handle(CreateContext(3, 10000 + (2 * SamplesPerPacket), tag: 9003));

        // Continue normally
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Verify original tags were preserved (packets 1-3 were already evicted before dups)
        for (ushort seq = 1; seq <= 3; seq++)
        {
            var p = receivedPackets.First(pkt => !pkt.Missed && pkt.SequenceNumber == seq);
            Assert.AreEqual(seq, p.Tag,
                $"Packet {seq} should retain the original tag ({seq}), not the delayed duplicate's ({9000 + seq}).");
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestDelayedDuplicateWithSmallBufferPreservesOriginalData()
    {
        // With a very small buffer (bufferSize=4) and no startup delay,
        // packets are evicted almost immediately. Duplicates arriving even
        // a few packets later should find their originals already evicted.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 10,   // bufferSize = 10*2/5 = 4
            StartupDuration = 0,
            ResynchronizationDuration = 100,
            ResynchronizationThreshold = 10,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;

        // p1 → p2 → p3 → p4 → dup(p1)
        handler.Handle(CreateContext(1, timestamp, tag: 1));
        timestamp += SamplesPerPacket;

        handler.Handle(CreateContext(2, timestamp, tag: 2));
        timestamp += SamplesPerPacket;

        handler.Handle(CreateContext(3, timestamp, tag: 3));
        timestamp += SamplesPerPacket;

        handler.Handle(CreateContext(4, timestamp, tag: 4));
        timestamp += SamplesPerPacket;

        // By now, packet 1 should be evicted (bufferSize=4, eviction starts immediately)
        handler.Handle(CreateContext(1, 10000, tag: 999));

        // Continue normally
        for (ushort i = 5; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var seq1 = receivedPackets.First(p => !p.Missed && p.SequenceNumber == 1);
        Assert.AreEqual(1, seq1.Tag,
            "Packet 1 should retain original tag (1), not delayed duplicate's (999).");

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestDuplicateOfLatestPacketDoesNotOverwriteOriginalData()
    {
        // Specifically targets the `sequenceNumberDiff is 0` fast-path
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        handler.Handle(CreateContext(1, timestamp, tag: 42));
        handler.Handle(CreateContext(1, timestamp, tag: 99)); // Duplicate with different tag

        timestamp += SamplesPerPacket;
        for (ushort i = 2; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var seq1 = receivedPackets.First(p => !p.Missed && p.SequenceNumber == 1);
        Assert.AreEqual(42, seq1.Tag,
            "Sequence 1 should retain the original tag, not the duplicate's.");
    }

    [TestMethod]
    public void TestDuplicateOfLatestPacketWithInterveningPacketsDoesNotOverwrite()
    {
        // Verifies that a duplicate of an earlier packet (not the latest) does not
        // overwrite the original after it has been evicted from the buffer.
        // With default config, packet 1 is evicted around seq 7.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Original packet 1
        handler.Handle(CreateContext(1, timestamp, tag: 42));
        timestamp += SamplesPerPacket;

        // Send enough intervening packets so packet 1 is evicted
        for (ushort i = 2; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Delayed duplicate of packet 1 with different tag (original already evicted)
        handler.Handle(CreateContext(1, 10000, tag: 999));

        // Continue the stream 11-30
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var seq1 = receivedPackets.First(p => !p.Missed && p.SequenceNumber == 1);
        Assert.AreEqual(42, seq1.Tag,
            "Sequence 1 should retain the original tag (42), not the delayed duplicate's (999).");

        // Also verify intervening packets were not affected
        for (ushort i = 2; i <= 6; i++)
        {
            var p = receivedPackets.First(pkt => !pkt.Missed && pkt.SequenceNumber == i);
            Assert.AreEqual(i, p.Tag,
                $"Packet {i} should retain its original tag.");
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    // ================================================================
    // Multiple SSRCs
    // ================================================================

    [TestMethod]
    public void TestMultipleSSRCsIndependent()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp1 = 10000;
        uint timestamp2 = 20000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp1, ssrc: 1000));
            handler.Handle(CreateContext(i, timestamp2, ssrc: 2000));
            timestamp1 += SamplesPerPacket;
            timestamp2 += SamplesPerPacket;
        }

        var ssrc1Packets = receivedPackets.Where(p => p.Ssrc == 1000 && !p.Missed).ToList();
        var ssrc2Packets = receivedPackets.Where(p => p.Ssrc == 2000 && !p.Missed).ToList();

        Assert.IsNotEmpty(ssrc1Packets, "SSRC 1000 should have received packets.");
        Assert.IsNotEmpty(ssrc2Packets, "SSRC 2000 should have received packets.");

        // Verify each SSRC stream is ordered
        for (int i = 1; i < ssrc1Packets.Count; i++)
            Assert.IsGreaterThan(ssrc1Packets[i - 1].SequenceNumber, ssrc1Packets[i].SequenceNumber);

        for (int i = 1; i < ssrc2Packets.Count; i++)
            Assert.IsGreaterThan(ssrc2Packets[i - 1].SequenceNumber, ssrc2Packets[i].SequenceNumber);
    }

    [TestMethod]
    public void TestMultipleSSRCsWithDifferentPatterns()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint ts;

        // SSRC 100: ordered
        ts = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, ts, ssrc: 100));
            ts += SamplesPerPacket;
        }

        // SSRC 200: pairwise swapped
        List<ushort> reordered = [2, 1, 4, 3, 6, 5, 8, 7, 10, 9];
        for (ushort i = 11; i <= 30; i++) reordered.Add(i);
        foreach (var seq in reordered)
        {
            handler.Handle(CreateContext(seq, 10000 + ((uint)(seq - 1) * SamplesPerPacket), ssrc: 200));
        }

        // SSRC 300: missing seq 5
        ts = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 5) { ts += SamplesPerPacket; continue; }
            handler.Handle(CreateContext(i, ts, ssrc: 300));
            ts += SamplesPerPacket;
        }

        var ssrc1 = receivedPackets.Where(p => p.Ssrc == 100 && !p.Missed).ToList();
        var ssrc2 = receivedPackets.Where(p => p.Ssrc == 200 && !p.Missed).ToList();

        Assert.IsNotEmpty(ssrc1, "SSRC 100 should deliver packets.");
        Assert.IsNotEmpty(ssrc2, "SSRC 200 should deliver packets.");

        // SSRC 300: single missing packet (seq 5) appears as a Lost event
        var ssrc3Seq5 = receivedPackets.FirstOrDefault(p => p.Ssrc == 300 && p.SequenceNumber == 5);
        Assert.IsTrue(ssrc3Seq5.Missed, "SSRC 300: seq 5 should be reported as a Lost event.");
    }

    [TestMethod]
    public void TestSsrcZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp, ssrc: 0));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        Assert.IsTrue(receivedPackets.All(p => p.Ssrc == 0), "All packets should have SSRC 0.");
    }

    [TestMethod]
    public void TestSsrcMaxValue()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp, ssrc: uint.MaxValue));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        Assert.IsTrue(receivedPackets.All(p => p.Ssrc == uint.MaxValue), "All packets should have SSRC uint.MaxValue.");
    }

    // ================================================================
    // Multiple Consecutive Missing Packets / Burst Loss
    // ================================================================

    [TestMethod]
    public void TestMultipleConsecutiveMissingPackets()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            if (i is >= 5 and <= 7)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // With 3 consecutive missing packets (5, 6, 7):
        // They are merged into a single Lost event of 2880 samples (3 × 960)
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 30, new HashSet<ushort> { 5, 6, 7 });
    }

    [TestMethod]
    public void TestBurstLossAndRecovery()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Normal flow: 1-10
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Burst loss: skip 11-20
        timestamp += 10 * SamplesPerPacket;

        // Recovery: 21-40
        for (ushort i = 21; i <= 40; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Burst of 10 missing (11-20): merged into 2 Lost events (3840 + 5760 samples)
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 40, new HashSet<ushort> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
    }

    [TestMethod]
    public void TestMissingLastPacketBeforeGap()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Send 1-29, skip 30
        for (ushort i = 1; i <= 29; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }
        timestamp += SamplesPerPacket; // Skip 30

        // Continue from 31
        for (ushort i = 31; i <= 50; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Seq 30 is a single missing packet; appears as a Lost event
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 50, new HashSet<ushort> { 30 });
    }

    // ================================================================
    // Late Arriving Packets
    // ================================================================

    [TestMethod]
    public void TestVeryLatePacketDoesNotCauseDuplicateDelivery()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 100; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Send packet 2 very late
        handler.Handle(CreateContext(2, 10000 + SamplesPerPacket));

        var seq2Count = receivedPackets.Count(p => !p.Missed && p.SequenceNumber == 2);
        Assert.AreEqual(1, seq2Count, "Late packet should not cause duplicate delivery of seq 2.");
    }

    [TestMethod]
    public void TestLatePacketWithinBufferWindow()
    {
        // Packet arrives late but buffer hasn't evicted its slot yet
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Send 1, 2, 4, 5 (skip 3), then send 3 late
        handler.Handle(CreateContext(1, timestampBase));
        handler.Handle(CreateContext(2, timestampBase + SamplesPerPacket));
        handler.Handle(CreateContext(4, timestampBase + (3 * SamplesPerPacket)));
        handler.Handle(CreateContext(5, timestampBase + (4 * SamplesPerPacket)));
        handler.Handle(CreateContext(3, timestampBase + (2 * SamplesPerPacket))); // Late arrival

        // Continue to flush
        for (ushort i = 6; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        // Seq 3 should ideally be delivered, not missed
        var seq3 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 3);
        Assert.IsFalse(seq3.Missed, "Packet 3 arrived late but within buffer window; should not be missed.");

        // All 30 should be delivered in order
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    // ================================================================
    // Configuration Validation
    // ================================================================

    [TestMethod]
    public void TestInvalidBufferDurationZero()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 0
        }));
    }

    [TestMethod]
    public void TestInvalidBufferDurationNegative()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = -1
        }));
    }

    [TestMethod]
    public void TestInvalidBufferDurationTooSmall()
    {
        // Duration=1 → bufferSize = 1*2/5 = 0 → invalid
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 1
        }));
    }

    [TestMethod]
    public void TestInvalidBufferDurationTwo()
    {
        // Duration=2 → bufferSize = 2*2/5 = 0 → invalid
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 2
        }));
    }

    [TestMethod]
    public void TestInvalidStartupDurationGreaterThanBuffer()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 100,
            StartupDuration = 200
        }));
    }

    [TestMethod]
    public void TestInvalidResynchronizationThresholdZero()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            ResynchronizationThreshold = 0
        }));
    }

    [TestMethod]
    public void TestInvalidResynchronizationThresholdNegative()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            ResynchronizationThreshold = -5
        }));
    }

    [TestMethod]
    public void TestInvalidIdleTimeoutZero()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            IdleTimeout = 0
        }));
    }

    [TestMethod]
    public void TestInvalidIdleTimeoutNegative()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            IdleTimeout = -1
        }));
    }

    [TestMethod]
    public void TestInvalidResynchronizationDurationTooSmall()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 240,
            ResynchronizationDuration = 50
        }));
    }

    [TestMethod]
    public void TestDefaultConfigurationValid()
    {
        var handler = new BufferedVoiceReceiveHandler();
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestNullConfigurationValid()
    {
        var handler = new BufferedVoiceReceiveHandler(null);
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestMinimalValidConfiguration()
    {
        // BufferDuration=3 → bufferSize = 3*2/5 = 1 (minimum positive)
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestStartupDurationEqualToBufferDuration()
    {
        // Edge: startup == buffer is allowed (startupSize <= bufferSize)
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 240,
            StartupDuration = 240
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestResynchronizationDurationEqualToBufferDuration()
    {
        // Edge: resync == buffer is allowed (resyncSamples >= bufferSamples)
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 240,
            ResynchronizationDuration = 240
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestMaxValidResynchronizationDuration()
    {
        // Max resynchronization before minResynchronizationPackets exceeds short.MaxValue:
        // 81919 * 2 / 5 = 32767 (== short.MaxValue) → valid
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            ResynchronizationDuration = 81919,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestResynchronizationDurationOverflowThrows()
    {
        // 81920 * 2 / 5 = 32768 (> short.MaxValue) → throws
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            ResynchronizationDuration = 81920,
        }));
    }

    [TestMethod]
    public void TestMaxValidBufferAndResynchronizationDuration()
    {
        // Largest possible BufferDuration with matching resync = 81919
        // (ResynchronizationDuration must be >= BufferDuration)
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 81919,
            ResynchronizationDuration = 81919,
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestBufferDurationOverflowViaResynchronizationThrows()
    {
        // BufferDuration = 81920 is technically valid for bufferSize/bufferSamples,
        // but requires ResynchronizationDuration >= 81920 which exceeds short.MaxValue threshold.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 81920,
            ResynchronizationDuration = 81920,
        }));
    }

    [TestMethod]
    public void TestBufferDurationCausesBufferSamplesOverflow()
    {
        // BufferDuration = 44739245 → bufferSize = 17895698 → bufferSamples overflows int
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 44739245,
            ResynchronizationDuration = 44739245,
        }));
    }

    [TestMethod]
    public void TestLargeIdleTimeout()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            IdleTimeout = int.MaxValue
        });
        Assert.IsNotNull(handler);
    }

    [TestMethod]
    public void TestLargeResynchronizationThreshold()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            ResynchronizationThreshold = int.MaxValue
        });
        Assert.IsNotNull(handler);
    }

    // ================================================================
    // Outlier Tracking
    // ================================================================

    [TestMethod]
    public void TestOutliersBelowThresholdNoResync()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 5,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Send 4 contiguous outliers (below threshold of 5)
        ushort outlierBase = 30000;
        uint outlierTs = 5000000;
        for (int i = 0; i < 4; i++)
        {
            handler.Handle(CreateContext((ushort)(outlierBase + i), outlierTs + (uint)(i * SamplesPerPacket)));
        }

        // Continue normal flow from 11
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // All 30 normal packets should be delivered in order (outliers are ignored)
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestOutlierCounterResetOnNonContiguousOutliers()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Two outliers from one range
        handler.Handle(CreateContext(30000, 5000000));
        handler.Handle(CreateContext(30001, 5000000 + SamplesPerPacket));

        // One outlier from a VERY different range (non-contiguous → should reset counter)
        handler.Handle(CreateContext(50000, 9000000));

        // One more contiguous with the new range
        handler.Handle(CreateContext(50001, 9000000 + SamplesPerPacket));

        // Normal flow continues — should not have resynced
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var normalPackets = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsNotEmpty(normalPackets, "Normal packets should still be delivered after non-contiguous outliers.");
    }

    [TestMethod]
    public void TestExactThresholdTriggersResync()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(2, timestamp + SamplesPerPacket));

        // Exactly 3 contiguous outliers → should trigger resync
        ushort jumpBase = 5000;
        uint jumpTs = 10000000;
        handler.Handle(CreateContext(jumpBase, jumpTs));
        handler.Handle(CreateContext((ushort)(jumpBase + 1), jumpTs + SamplesPerPacket));
        handler.Handle(CreateContext((ushort)(jumpBase + 2), jumpTs + (2 * SamplesPerPacket)));

        // Continue from new range
        for (ushort i = 3; i <= 25; i++)
        {
            handler.Handle(CreateContext((ushort)(jumpBase + i), jumpTs + (i * SamplesPerPacket)));
        }

        // Old packets should have been force-evicted
        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync.");

        var seq2 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 2);
        Assert.IsFalse(seq2.Missed, "Sequence 2 should have been force-evicted during resync.");

        // New range should all be delivered
        var newRange = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpBase).ToList();
        Assert.IsNotEmpty(newRange, "New range packets should be delivered after resync.");
    }

    [TestMethod]
    public void TestBelowThresholdByOneDoesNotResync()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 4,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // 3 contiguous outliers (threshold is 4 → should NOT trigger resync)
        ushort outlierBase = 30000;
        uint outlierTs = 5000000;
        handler.Handle(CreateContext(outlierBase, outlierTs));
        handler.Handle(CreateContext((ushort)(outlierBase + 1), outlierTs + SamplesPerPacket));
        handler.Handle(CreateContext((ushort)(outlierBase + 2), outlierTs + (2 * SamplesPerPacket)));

        // Resume normal
        for (ushort i = 6; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Normal stream should continue uninterrupted
        var normalDelivered = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsNotEmpty(normalDelivered, "Normal flow should continue when outliers are below threshold.");
    }

    // ================================================================
    // Idle Timeout / Timer Behavior
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TestIdleTimeoutForceEvictsBuffered()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for the buffer timer to fire and force-evict
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // All 5 packets should be force-evicted by the timer
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 5);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestIdleTimeoutThenNewStream()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
            ResynchronizationDuration = 100,
            IdleTimeout = 200,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for idle timeout + disposal
        await Task.Delay(600, context.CancellationToken).ConfigureAwait(false);

        int countAfterIdle = receivedPackets.Count;
        Assert.IsGreaterThan(0, countAfterIdle, "Packets should be force-evicted during idle.");

        // New stream — handler should reinitialize state
        for (ushort i = 100; i <= 120; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        await Task.Delay(200, context.CancellationToken).ConfigureAwait(false);

        Assert.IsGreaterThan(countAfterIdle, receivedPackets.Count, "New packets should be handled after idle timeout and reinit.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestForceEvictAllOnTimerExactPackets()
    {
        // Verifies that the timer fires and emits ALL buffered packets in exact order,
        // including marking any gaps as missed.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        // Send 3 packets with a gap (missing seq 2)
        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(3, timestamp + (2 * SamplesPerPacket)));
        handler.Handle(CreateContext(4, timestamp + (3 * SamplesPerPacket)));

        // No eviction should have happened yet (startup == buffer)
        Assert.IsEmpty(receivedPackets, "No packets should be emitted before timer fires.");

        // Wait for the buffer timer to fire
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // Timer should have force-evicted all: 1, 2(Lost), 3, 4
        // Missing packet (seq 2) appears as a Lost event.
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 4, new HashSet<ushort> { 2 });
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestIdleTimeoutRemovesState()
    {
        // After the first timer fire (ForceEvictAll → Idle),
        // a second timer fire (idle timeout) removes the state.
        // Sending new packets should re-initialize cleanly.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 100,
            StartupDuration = 0,
            ResynchronizationDuration = 100,
            IdleTimeout = 200,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(2, timestamp + SamplesPerPacket));

        // Wait for buffer timer (100ms) + idle timer (200ms) + safety margin
        await Task.Delay(600, context.CancellationToken).ConfigureAwait(false);

        int countAfterRemoval = receivedPackets.Count;

        // State should be removed. Sending new packets should create fresh state.
        handler.Handle(CreateContext(500, 5000000));
        handler.Handle(CreateContext(501, 5000000 + SamplesPerPacket));

        await Task.Delay(400, context.CancellationToken).ConfigureAwait(false);

        // New packets should be handled independently (fresh initialization)
        var newPackets = receivedPackets.Skip(countAfterRemoval)
            .Where(p => !p.Missed && (p.SequenceNumber == 500 || p.SequenceNumber == 501))
            .ToList();
        Assert.IsNotEmpty(newPackets, "New packets should be handled after state removal and re-initialization.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestTimerResetOnNewPacket()
    {
        // Sending new packets should reset the timer, preventing force-eviction
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));

        // Send packets at regular intervals, each resetting the timer before the 200ms fires
        for (ushort i = 2; i <= 5; i++)
        {
            await Task.Delay(50, context.CancellationToken).ConfigureAwait(false);
            timestamp += SamplesPerPacket;
            handler.Handle(CreateContext(i, timestamp));
        }

        // The timer should not have fired yet (last reset was ~50ms ago, timer is 200ms)
        Assert.IsEmpty(receivedPackets, "Timer should not have fired while packets keep arriving within the interval.");

        // Now wait for the timer to fire
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // All 5 packets should now be force-evicted
        Assert.HasCount(5, receivedPackets, "All 5 packets should be emitted after timer fires.");
    }

    // ================================================================
    // Edge: Single Packet
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TestSinglePacket()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
        });

        handler.Handle(CreateContext(1, 10000));

        await Task.Delay(200, context.CancellationToken).ConfigureAwait(false);

        Assert.IsNotEmpty(receivedPackets, "Single packet should be eventually emitted.");
        Assert.AreEqual(1, receivedPackets.First(p => !p.Missed).SequenceNumber);
    }

    // ================================================================
    // Edge: Reorder Patterns
    // ================================================================

    [TestMethod]
    public void TestAlternatingPairSwap()
    {
        // Pattern: 2,1, 4,3, 6,5, 8,7, ...
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;
        List<ushort> sendOrder = [];

        for (ushort i = 1; i <= 30; i += 2)
        {
            ushort next = (ushort)(i + 1);
            if (next <= 30) sendOrder.Add(next);
            sendOrder.Add(i);
        }

        foreach (var seq in sendOrder)
        {
            handler.Handle(CreateContext(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        // Buffer should reorder all pair swaps to produce exact 1-30 delivery
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestCompletelyReversedPackets()
    {
        // A fully reversed stream exceeds the buffer's reordering window, so some packets
        // may be emitted out of order or missed. The handler should not crash.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        for (ushort i = 30; i >= 1; i--)
        {
            handler.Handle(CreateContext(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        Assert.IsNotEmpty(receivedPackets);

        // Only verify no crash and that some packets were delivered — full reorder
        // guarantee is not possible when all packets arrive in reverse order
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.IsNotEmpty(nonMissed, "At least some packets should be delivered.");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestRandomReorderingWithJitter()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;
        var rng = new Random(42); // Fixed seed for reproducibility

        List<ushort> seqs = [];
        for (ushort i = 1; i <= 200; i++)
            seqs.Add(i);

        // Shuffle with limited displacement (simulate realistic network jitter)
        for (int i = 0; i < seqs.Count; i++)
        {
            int swapTarget = Math.Clamp(i + rng.Next(-2, 3), 0, seqs.Count - 1);
            (seqs[i], seqs[swapTarget]) = (seqs[swapTarget], seqs[i]);
        }

        foreach (var seq in seqs)
        {
            handler.Handle(CreateContext(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        // Wait for remaining buffered packets to be force-evicted
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 200);
    }

    [TestMethod]
    public void TestMissingAndReorderedCombined()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Pattern: 1, 3, 2, (skip 4), 6, 5, 7, 8, (skip 9), 11, 10, 12-30
        List<ushort> sendOrder = [1, 3, 2, 6, 5, 7, 8, 11, 10];
        for (ushort i = 12; i <= 30; i++) sendOrder.Add(i);

        foreach (var seq in sendOrder)
        {
            handler.Handle(CreateContext(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        // 30 entries: 28 delivered + 2 lost (missing packets 4 and 9)
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 30, new HashSet<ushort> { 4, 9 });
    }

    // ================================================================
    // Resynchronization Scenarios
    // ================================================================

    [TestMethod]
    public void TestDoubleResynchronization()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 2,
        });

        uint timestamp = 10000;

        // First normal flow
        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // First jump
        ushort jump1 = 10000;
        uint jump1Ts = 20000000;
        handler.Handle(CreateContext(jump1, jump1Ts));
        handler.Handle(CreateContext((ushort)(jump1 + 1), jump1Ts + SamplesPerPacket));

        for (ushort i = 2; i <= 5; i++)
            handler.Handle(CreateContext((ushort)(jump1 + i), jump1Ts + (i * SamplesPerPacket)));

        // Second jump
        ushort jump2 = 50000;
        uint jump2Ts = 80000000;
        handler.Handle(CreateContext(jump2, jump2Ts));
        handler.Handle(CreateContext((ushort)(jump2 + 1), jump2Ts + SamplesPerPacket));

        for (ushort i = 2; i <= 25; i++)
            handler.Handle(CreateContext((ushort)(jump2 + i), jump2Ts + (i * SamplesPerPacket)));

        var range1 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 5).ToList();
        var range2 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jump1 && p.SequenceNumber <= jump1 + 5).ToList();
        var range3 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jump2).ToList();

        Assert.IsNotEmpty(range1, "First range should be force-evicted and delivered.");
        Assert.IsNotEmpty(range2, "Second range should be force-evicted and delivered.");
        Assert.IsNotEmpty(range3, "Third range should be delivered.");

        // Verify exact count of first range (all 5 force-evicted)
        Assert.HasCount(5, range1, "All 5 packets from first range should be force-evicted.");
    }

    [TestMethod]
    public void TestResyncAcrossSequenceWraparound()
    {
        // Resync triggered near ushort boundary
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 2,
        });

        uint timestamp = 10000;

        // Start near max range
        ushort startSeq = 65530;
        for (int i = 0; i < 5; i++)
        {
            handler.Handle(CreateContext((ushort)(startSeq + i), timestamp));
            timestamp += SamplesPerPacket;
        }

        // Jump across the wraparound boundary to trigger resync
        ushort jumpSeq = 100;
        uint jumpTs = 50000000;
        handler.Handle(CreateContext(jumpSeq, jumpTs));
        handler.Handle(CreateContext((ushort)(jumpSeq + 1), jumpTs + SamplesPerPacket));

        for (ushort i = 2; i <= 25; i++)
            handler.Handle(CreateContext((ushort)(jumpSeq + i), jumpTs + (i * SamplesPerPacket)));

        // Old range should have been force-evicted
        var preJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= startSeq).ToList();
        Assert.HasCount(5, preJump, "All 5 pre-jump packets should be force-evicted.");

        var postJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpSeq && p.SequenceNumber < jumpSeq + 26).ToList();
        Assert.IsNotEmpty(postJump, "Packets after resync across wraparound should be delivered.");
    }

    // ================================================================
    // Buffer Size Boundary
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TestExactBufferSizeBoundary()
    {
        // Default buffer size = 240*2/5 = 96
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 96; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // One more to trigger overflow
        handler.Handle(CreateContext(97, timestamp));

        // Wait for remaining buffered packets to be force-evicted
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 97);
    }

    [TestMethod]
    public async Task TestMinimumBufferSize()
    {
        // BufferDuration=3 → bufferSize=1
        var (handler, receivedPackets) = InitializeHandler(new()
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
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    // ================================================================
    // Zero Startup Duration
    // ================================================================

    [TestMethod]
    public void TestZeroStartupDuration()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 240,
            StartupDuration = 0,
        });

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // With startup=0, eviction starts from the very first packet
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    // ================================================================
    // Random Packet Loss
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TestRandomPacketLoss10Percent()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        var rng = new Random(123);
        HashSet<ushort> dropped = [];

        for (ushort i = 1; i <= 200; i++)
        {
            if (rng.NextDouble() < 0.1) // 10% loss
            {
                dropped.Add(i);
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for remaining buffered packets to be force-evicted
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 200, dropped);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestRandomPacketLoss50Percent()
    {
        // Extreme loss to stress the handler
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        var rng = new Random(999);
        HashSet<ushort> dropped = [];

        for (ushort i = 1; i <= 200; i++)
        {
            if (rng.NextDouble() < 0.5)
            {
                dropped.Add(i);
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for remaining buffered packets to be force-evicted
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 200, dropped);
    }

    // ================================================================
    // Timestamp Inconsistencies
    // ================================================================

    [TestMethod]
    public async Task TestInconsistentTimestampGaps()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket + i;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        Assert.HasCount(1, receivedPackets, "With inconsistent timestamps, packets should be treated as outliers, so only the first packet is accepted and the rest are ignored.");
    }

    [TestMethod]
    public void TestSameTimestampDifferentSequences()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Multiple packets sharing the same timestamp (unusual but shouldn't crash)
        for (ushort i = 6; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
        }

        for (ushort i = 11; i <= 30; i++)
        {
            timestamp += SamplesPerPacket;
            handler.Handle(CreateContext(i, timestamp));
        }

        // No crash = success; behaviour under malformed timestamp is implementation-defined
        Assert.IsNotEmpty(receivedPackets);
    }

    [TestMethod]
    public void TestTimestampGoesBackward()
    {
        // Packet with lower timestamp than previous (clock skew / malformed)
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Seq 6 with a timestamp lower than expected (backward)
        handler.Handle(CreateContext(6, 10000));

        // Continue normally
        for (ushort i = 7; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Should not crash
        Assert.IsNotEmpty(receivedPackets);
    }

    [TestMethod]
    public void TestInconsistentTimestampPacketIsIgnored()
    {
        // The handler checks timestampDiff % 120 (MinSamplesPerPacket).
        // Packets with timestamps that aren't aligned to this boundary are treated as outliers.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 100, // High threshold to prevent resync
        });

        uint timestamp = 10000;

        // Normal stream: 1-10
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }
        // timestamp = 19600, latestTs = 18640

        // Packets with inconsistent timestamps (not multiples of 120 from latest)
        // 961 % 120 = 1 ≠ 0 → outlier
        handler.Handle(CreateContext(11, 18640 + 961));
        handler.Handle(CreateContext(12, 18640 + 961 + SamplesPerPacket));
        handler.Handle(CreateContext(13, 18640 + 961 + (2 * SamplesPerPacket)));

        // Continue normal stream from 11 with correct timestamps
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // All 30 packets should be delivered in order.
        // The inconsistent-timestamp packets were treated as outliers (ignored),
        // so the correctly-timestamped seq 11-30 are accepted instead.
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestInconsistentTimestampTriggersResyncAtThreshold()
    {
        // When enough consecutive packets with inconsistent timestamps arrive (and they're
        // consistent with each other), they trigger resynchronization.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        // Normal stream: 1-5
        for (ushort i = 1; i <= 5; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }
        // timestamp = 14800, latestTs = 14800 - 960 = 13840

        // 3 outliers that are consistent with each other but not with the main stream
        // offset of 1 from 120 boundary → outlier relative to main stream
        // but diff between them is 960 (multiple of 120) → they form a contiguous chain
        uint outlierBase = 5000000;
        handler.Handle(CreateContext(500, outlierBase));
        handler.Handle(CreateContext(501, outlierBase + SamplesPerPacket));
        handler.Handle(CreateContext(502, outlierBase + (2 * SamplesPerPacket)));

        // Resync should have triggered — old packets force-evicted.
        // Continue from new range.
        for (ushort i = 3; i <= 25; i++)
        {
            handler.Handle(CreateContext((ushort)(500 + i), outlierBase + (i * SamplesPerPacket)));
        }

        // Old packets 1-5 should have been force-evicted
        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync.");

        // New range should be delivered
        var newRange = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 500).ToList();
        Assert.IsNotEmpty(newRange, "New range packets should be delivered after resync.");
    }

    // ================================================================
    // API / Property Tests
    // ================================================================

    [TestMethod]
    public void TestRequiresExternalSocketAddress()
    {
        var handler = new BufferedVoiceReceiveHandler();
        Assert.IsTrue(handler.RequiresExternalSocketAddress);
    }

    [TestMethod]
    public void TestNoEventHandlerRegisteredNoCrash()
    {
        var handler = new BufferedVoiceReceiveHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // No crash = pass
    }

    // ================================================================
    // CanCorrectLoss Behavior
    // ================================================================

    [TestMethod]
    public void TestLostEventAfterSingleMissingPacket()
    {
        // When a single packet is lost, it should appear as a Lost event.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 10)
            {
                timestamp += SamplesPerPacket;
                continue; // Skip seq 10
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Seq 10 should appear as a Lost event
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 30, new HashSet<ushort> { 10 });
    }

    [TestMethod]
    public void TestLostEventsAfterMultipleConsecutiveMissing()
    {
        // When multiple consecutive packets are lost, they are merged into a single Lost event.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i is >= 10 and <= 13)
            {
                timestamp += SamplesPerPacket;
                continue; // Skip seq 10-13 (4 consecutive missing)
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // 4 consecutive missing packets are merged into a single Lost event of 3840 samples (4 × 960)
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 30, new HashSet<ushort> { 10, 11, 12, 13 });
    }

    [TestMethod]
    public void TestNoLostEventsForOrderedStream()
    {
        // In a perfectly ordered stream with no loss, no packet should be reported as missed.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // In a perfectly ordered stream with no loss, no packet should be reported as missed.
        // Also verifies exact timestamps.
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    [TestMethod]
    public void TestLostEventsWithMultipleGaps()
    {
        // Multiple separate gaps should each produce Lost event(s).
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        HashSet<ushort> skipped = [5, 15, 25];

        for (ushort i = 1; i <= 40; i++)
        {
            if (skipped.Contains(i))
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Each separate gap should produce a Lost event
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 40, skipped);
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestLostEventOnForceEvictAll()
    {
        // When the timer fires and ForceEvictAll runs, missing packets
        // should be emitted as Lost events (merged if consecutive).
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        // Send 3 packets with a gap (missing seq 2)
        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(3, timestamp + (2 * SamplesPerPacket)));
        handler.Handle(CreateContext(4, timestamp + (3 * SamplesPerPacket)));

        Assert.IsEmpty(receivedPackets, "No packets should be emitted before timer fires.");

        // Wait for the buffer timer to fire (ForceEvictAll)
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // Timer should emit: 1, 2(Lost), 3, 4
        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, SamplesPerPacket, 4, new HashSet<ushort> { 2 });
    }

    // ================================================================
    // FEC (Forward Error Correction) Data Tests
    // ================================================================

    [TestMethod]
    public void TestFecDataOnSingleMissingPacket()
    {
        // When a single packet is lost, the lost event should have DecodeFec = true
        // with FEC data from the next delivered packet's frame.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 5)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var lost5 = receivedPackets.First(p => p.Missed);
        Assert.IsTrue(lost5.DecodeFec, "Single lost packet should have DecodeFec set.");
        Assert.AreEqual(6, lost5.FecTag, "FEC data should contain the frame of the next delivered packet (seq 6).");
    }

    [TestMethod]
    public void TestFecDataOnConsecutiveMissingPackets()
    {
        // When multiple consecutive packets are lost and merged into a single event,
        // the merged event should have DecodeFec = true with FEC data from the next packet.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i is >= 5 and <= 7)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var lostEvents = receivedPackets.Where(p => p.Missed).ToList();
        Assert.HasCount(1, lostEvents, "3 consecutive missing packets should merge into 1 lost event.");
        Assert.IsTrue(lostEvents[0].DecodeFec, "Merged lost event should have DecodeFec set.");
        Assert.AreEqual(8, lostEvents[0].FecTag, "FEC data should contain the frame of seq 8.");
    }

    [TestMethod]
    public void TestFecDataOnMultipleGaps()
    {
        // Two separate gaps should each produce a lost event with DecodeFec = true,
        // each containing FEC data from their respective next delivered packet.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        HashSet<ushort> skipped = [5, 15];
        for (ushort i = 1; i <= 30; i++)
        {
            if (skipped.Contains(i))
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var lostEvents = receivedPackets.Where(p => p.Missed).ToList();
        Assert.HasCount(2, lostEvents, "Two separate single-packet gaps should produce 2 lost events.");

        Assert.IsTrue(lostEvents[0].DecodeFec, "First lost event should have DecodeFec.");
        Assert.AreEqual(6, lostEvents[0].FecTag, "First gap FEC should come from seq 6.");

        Assert.IsTrue(lostEvents[1].DecodeFec, "Second lost event should have DecodeFec.");
        Assert.AreEqual(16, lostEvents[1].FecTag, "Second gap FEC should come from seq 16.");
    }

    [TestMethod]
    public void TestFecDataOnLargeBurstLoss()
    {
        // 10 consecutive missing packets produce multiple merged lost events.
        // Only the last merged event should have DecodeFec = true.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 40; i++)
        {
            if (i is >= 11 and <= 20)
            {
                timestamp += SamplesPerPacket;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var lostEvents = receivedPackets.Where(p => p.Missed).ToList();
        Assert.IsGreaterThan(1, lostEvents.Count, "Large burst loss should produce multiple merged events.");

        for (int i = 0; i < lostEvents.Count - 1; i++)
            Assert.IsFalse(lostEvents[i].DecodeFec, $"Non-last merged lost event {i} should not have DecodeFec.");

        Assert.IsTrue(lostEvents[^1].DecodeFec, "Last merged lost event should have DecodeFec.");
        Assert.AreEqual(21, lostEvents[^1].FecTag, "FEC data should come from seq 21 (next delivered packet).");
    }

    [DoNotParallelize]
    [TestMethod]
    public async Task TestFecDataOnForceEvict()
    {
        // When the timer fires and force-evicts packets with a gap,
        // the lost event should have FEC data from the next stored packet.
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            StartupDuration = 200,
            ResynchronizationDuration = 400,
            IdleTimeout = 5000,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp, tag: 1));
        handler.Handle(CreateContext(3, timestamp + (2 * SamplesPerPacket), tag: 3));
        handler.Handle(CreateContext(4, timestamp + (3 * SamplesPerPacket), tag: 4));

        Assert.IsEmpty(receivedPackets);

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // Force-evicted: 1, 2(Lost with FEC from seq 3), 3, 4
        var lost2 = receivedPackets.First(p => p.Missed);
        Assert.IsTrue(lost2.DecodeFec, "Force-evicted lost packet should have DecodeFec.");
        Assert.AreEqual(3, lost2.FecTag, "FEC data should contain the frame of seq 3.");
    }

    [TestMethod]
    public void TestFecDataNotPresentOnDeliveredPackets()
    {
        // In a stream with no loss, all delivered packets should have DecodeFec = false.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        foreach (var p in receivedPackets)
        {
            Assert.IsFalse(p.Missed, $"Packet {p.SequenceNumber} should not be missed.");
            Assert.IsFalse(p.DecodeFec, $"Delivered packet {p.SequenceNumber} should not have DecodeFec set.");
        }
    }

    // ================================================================
    // Edge: Scattered outliers that don't form a stream
    // ================================================================

    [TestMethod]
    public void TestScatteredOutliersFollowedByNormalFlow()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        // First packet initializes state
        handler.Handle(CreateContext(1, timestamp));

        // Send widely separated outliers that never form a contiguous stream
        handler.Handle(CreateContext(20000, 50000000));
        handler.Handle(CreateContext(40000, 90000000));
        handler.Handle(CreateContext(60000, 130000000));

        // Resume normal flow
        for (ushort i = 2; i <= 30; i++)
        {
            timestamp += SamplesPerPacket;
            handler.Handle(CreateContext(i, timestamp));
        }

        Assert.IsNotEmpty(receivedPackets);

        // Normal-range packets should all be delivered
        var normalDelivered = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsNotEmpty(normalDelivered, "Normal-range packets should be delivered despite scattered outliers.");
    }

    // ================================================================
    // Edge: Large volume
    // ================================================================

    [DoNotParallelize]
    [TestMethod]
    public async Task TestLargePacketVolume()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10000; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;

            if (i == 10000) break;
        }

        // Wait for remaining buffered packets to be force-evicted
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 10000);
    }

    [TestMethod]
    public void TestFullSequenceNumberCycle()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 0;

        for (int i = 0; i < 65536; i++)
        {
            handler.Handle(CreateContext((ushort)i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        // Verify all 65536 packets delivered in order with no misses
        Assert.HasCount(65536, receivedPackets, "All 65536 packets should be delivered.");

        ushort last = 0;
        bool first = true;
        foreach (var p in receivedPackets)
        {
            Assert.IsFalse(p.Missed, $"Packet {p.SequenceNumber} should not be missed.");
            if (!first)
            {
                ushort diff = (ushort)(p.SequenceNumber - last);
                Assert.IsGreaterThan((ushort)0, diff, $"Non-monotonic: {last} -> {p.SequenceNumber}");
                Assert.IsLessThan((ushort)32768, diff, $"Wrapped incorrectly: {last} -> {p.SequenceNumber}");
            }
            last = p.SequenceNumber;
            first = false;
        }
    }

    // ================================================================
    // Edge: Resync with minimum threshold = 1
    // ================================================================

    [TestMethod]
    public void TestResyncThresholdOne()
    {
        // With threshold=1, a single outlier triggers immediate resync
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 3,
            StartupDuration = 0,
            ResynchronizationDuration = 3,
            ResynchronizationThreshold = 1,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;

        handler.Handle(CreateContext(1, timestamp));
        handler.Handle(CreateContext(2, timestamp + SamplesPerPacket));

        // Single outlier → should trigger resync
        ushort jumpSeq = 500;
        uint jumpTs = 5000000;
        handler.Handle(CreateContext(jumpSeq, jumpTs));

        // Continue from new range
        for (ushort i = 1; i <= 25; i++)
        {
            handler.Handle(CreateContext((ushort)(jumpSeq + i), jumpTs + (i * SamplesPerPacket)));
        }

        // Old packets 1 and 2 should be force-evicted
        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync.");

        var seq2 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 2);
        Assert.IsFalse(seq2.Missed, "Sequence 2 should have been force-evicted during resync.");

        var postJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpSeq).ToList();
        Assert.IsNotEmpty(postJump, "Single outlier with threshold=1 should trigger resync.");
    }

    // ================================================================
    // Edge: Rapid fire packets with same seq but different timestamps
    // ================================================================

    [TestMethod]
    public void TestSameSequenceNumberDifferentTimestamps()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Send a normal stream
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Re-send seq 5 with a different timestamp (replay attack or corruption)
        handler.Handle(CreateContext(5, timestamp + 50000));

        // Continue normally
        for (ushort i = 11; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Should not crash; the replayed packet should be handled gracefully
        Assert.IsNotEmpty(receivedPackets);
    }

    // ================================================================
    // Edge: Packets arriving in groups (bursty arrival)
    // ================================================================

    [TestMethod]
    public void TestBurstyArrival()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Simulate bursty network: packets arrive in groups of 5
        for (int burst = 0; burst < 6; burst++)
        {
            // Each burst delivers 5 packets at once but with correct timestamps
            int startIdx = burst * 5;
            for (int j = 0; j < 5; j++)
            {
                ushort seq = (ushort)(startIdx + j + 1);
                handler.Handle(CreateContext(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
            }
        }

        // All 30 packets should be delivered in exact order
        AssertExactOrderedDelivery(receivedPackets, 1, 10000, SamplesPerPacket, 30);
    }

    // ================================================================
    // Edge: All packets missing except boundaries
    // ================================================================

    [TestMethod]
    public void TestSparsePackets()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Only send every 5th packet: 1, 6, 11, 16, 21, 26
        for (ushort i = 1; i <= 30; i += 5)
        {
            handler.Handle(CreateContext(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        // The handler should still function; gaps between packets are large but within buffer range
        Assert.IsNotEmpty(receivedPackets);

        // Verify all non-missed packets are among the sent ones
        var sent = new HashSet<ushort> { 1, 6, 11, 16, 21, 26 };
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        foreach (var p in nonMissed)
        {
            Assert.IsTrue(sent.Contains(p.SequenceNumber),
                $"Received packet {p.SequenceNumber} that was never sent.");
        }
    }

    // ================================================================
    // Variable Frame Sizes (2.5 ms to 120 ms)
    // ================================================================

    // Valid Opus frame sizes and their sample counts at 48kHz:
    //   2.5 ms →  120 samples
    //   5   ms →  240 samples
    //  10   ms →  480 samples
    //  20   ms →  960 samples (default)
    //  40   ms → 1920 samples
    //  60   ms → 2880 samples
    // 120   ms → 5760 samples

    [TestMethod]
    public void TestFrameSize2_5ms()
    {
        const uint samplesPerFrame = 120; // 2.5 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 50; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 50);
    }

    [TestMethod]
    public void TestFrameSize5ms()
    {
        const uint samplesPerFrame = 240; // 5 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 50; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 50);
    }

    [TestMethod]
    public void TestFrameSize10ms()
    {
        const uint samplesPerFrame = 480; // 10 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 50; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 50);
    }

    [TestMethod]
    public void TestFrameSize40ms()
    {
        const uint samplesPerFrame = 1920; // 40 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 30);
    }

    [TestMethod]
    public void TestFrameSize60ms()
    {
        const uint samplesPerFrame = 2880; // 60 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 30);
    }

    [TestMethod]
    public void TestFrameSize120ms()
    {
        const uint samplesPerFrame = 5760; // 120 ms at 48kHz

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += samplesPerFrame;
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 30);
    }

    [TestMethod]
    public void TestFrameSizeChangeImmediately()
    {
        // Stream changes frame size mid-stream:
        // First 10 packets at 20ms (960 samples), then 10 at 10ms (480), then 10 at 40ms (1920).
        // The handler should accept all since each diff is a multiple of 120.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += 960; // 20 ms
        }

        for (ushort i = 11; i <= 20; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += 480; // 10 ms
        }

        for (ushort i = 21; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamp));
            timestamp += 1920; // 40 ms
        }

        // All 30 packets delivered. Check timestamps individually.
        Assert.HasCount(30, receivedPackets, "All 30 packets should be delivered.");
        uint expectedTs = 10000;
        for (int i = 0; i < 30; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} was marked as missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {i}.");
            Assert.AreEqual(expectedTs, p.Timestamp, $"Expected timestamp {expectedTs} at position {i}, got {p.Timestamp}.");

            if (i < 10)
                expectedTs += 960;
            else if (i < 20)
                expectedTs += 480;
            else
                expectedTs += 1920;
        }
    }

    [TestMethod]
    public void TestFrameSizeChangeEveryPacket()
    {
        // Alternating frame sizes every packet: 2.5ms (120), 5ms (240), 10ms (480), 20ms (960), repeat
        // All are multiples of 120, so should be accepted.
        uint[] frameSizes = [120, 240, 480, 960];

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] expectedTimestamps = new uint[30];

        for (ushort i = 1; i <= 30; i++)
        {
            expectedTimestamps[i - 1] = timestamp;
            handler.Handle(CreateContext(i, timestamp));
            timestamp += frameSizes[(i - 1) % frameSizes.Length];
        }

        Assert.HasCount(30, receivedPackets, "All 30 packets should be delivered.");
        for (int i = 0; i < 30; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} was marked as missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(expectedTimestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }
    }

    [TestMethod]
    public void TestFrameSizeChangeWithReordering()
    {
        // Reordered packets with varying frame sizes.
        // Send order: 1, 3, 2, 4, 5, 6-30 in order.
        // All use different frame sizes but all multiples of 120.
        uint[] frameSizes = [960, 480, 240, 120, 1920, 2880]; // cycling

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += frameSizes[i % frameSizes.Length];
        }

        // Send out of order: 1, 3, 2, then 4-30
        handler.Handle(CreateContext(1, timestamps[0]));
        handler.Handle(CreateContext(3, timestamps[2]));
        handler.Handle(CreateContext(2, timestamps[1]));

        for (ushort i = 4; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamps[i - 1]));
        }

        Assert.HasCount(30, receivedPackets, "All 30 packets should be delivered.");
        for (int i = 0; i < 30; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} was marked as missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(timestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }
    }

    [TestMethod]
    public async Task TestFrameSizeChangeWithMissingPackets()
    {
        // Variable frame sizes with packet loss. Verifies lost event timestamps are correct.
        // Frame sizes: 20ms (960) for first 10, then 10ms (480) for next 20.
        // Missing: packets 5 (during 20ms phase) and 15 (during 10ms phase).
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += (uint)(i < 10 ? 960 : 480);
        }

        for (ushort i = 1; i <= 30; i++)
        {
            if (i is 5 or 15)
                continue; // Skip these
            handler.Handle(CreateContext(i, timestamps[i - 1], frameSamples: i <= 10 ? 960 : 480));
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // Verify all 30 entries including lost events with correct timestamps
        Assert.HasCount(30, receivedPackets, "Should have 30 entries (28 delivered + 2 lost).");
        for (int i = 0; i < 30; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {i}.");
            Assert.AreEqual(timestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");

            if (expectedSeq is 5 or 15)
            {
                Assert.IsTrue(p.Missed, $"Packet {expectedSeq} should be marked as missed.");
                Assert.IsTrue(p.DecodeFec, $"Lost packet {expectedSeq} should have DecodeFec set.");
            }
            else
            {
                Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
                Assert.IsFalse(p.DecodeFec, $"Delivered packet {expectedSeq} should not have DecodeFec set.");
            }
        }
    }

    [TestMethod]
    public async Task TestFrameSizeChangeDuringBurstLoss()
    {
        // Burst loss happens exactly at the frame size transition boundary.
        // Packets 1-10: 20ms (960 samples)
        // Packets 11-15: LOST (burst loss spanning the transition)
        // Packets 16-30: 10ms (480 samples)
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[30];
        for (int i = 0; i < 30; i++)
        {
            timestamps[i] = timestamp;
            timestamp += (uint)(i < 10 ? 960 : 480);
        }

        // Send packets 1-10 at 20ms
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamps[i - 1], frameSamples: 960));
        }

        // Skip 11-15 (burst loss at transition boundary)

        // Send packets 16-30 at 10ms
        for (ushort i = 16; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestamps[i - 1], frameSamples: 480));
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // The 5 lost packets (11-15) are merged into a single lost event.
        // Total lost samples: timestamps[15] - (timestamps[9] + 960) = 22000 - 19600 = 2400
        Assert.HasCount(26, receivedPackets, "Should have 26 entries (25 delivered + 1 merged lost).");

        // Verify first 10 delivered packets
        for (int i = 0; i < 10; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {i}.");
            Assert.AreEqual(timestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }

        // Verify merged lost event for packets 11-15
        {
            var p = receivedPackets[10];
            Assert.IsTrue(p.Missed, "Packets 11-15 should appear as a merged lost event.");
            Assert.AreEqual(timestamps[10], p.Timestamp, "Lost event should start at timestamp of first missing packet.");
            Assert.AreEqual(2400, p.SamplesPerChannel, "Merged lost event should cover 2400 samples.");
            Assert.IsTrue(p.DecodeFec, "Merged lost event should have DecodeFec set (FEC from next delivered packet).");
        }

        // Verify remaining delivered packets 16-30
        for (int i = 0; i < 15; i++)
        {
            var p = receivedPackets[11 + i];
            var expectedSeq = (ushort)(16 + i);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {11 + i}.");
            Assert.AreEqual(timestamps[15 + i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }
    }

    [TestMethod]
    public async Task TestFrameSize2_5msWithMissing()
    {
        // Smallest valid frame size (2.5 ms = 120 samples) with packet loss.
        const uint samplesPerFrame = 120;

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 50; i++)
        {
            if (i is 10 or 25 or 40)
            {
                timestamp += samplesPerFrame;
                continue;
            }
            handler.Handle(CreateContext(i, timestamp, frameSamples: (int)samplesPerFrame));
            timestamp += samplesPerFrame;
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        AssertExactSequenceWithLoss(receivedPackets, 1, 10000, samplesPerFrame, 50, new HashSet<ushort> { 10, 25, 40 });
    }

    [TestMethod]
    public void TestFrameSize120msWithReordering()
    {
        // Largest valid Opus frame size (120 ms = 5760 samples) with reordering.
        const uint samplesPerFrame = 5760;

        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Reordered: 1, 3, 2, 4-30
        handler.Handle(CreateContext(1, timestampBase));
        handler.Handle(CreateContext(3, timestampBase + (2 * samplesPerFrame)));
        handler.Handle(CreateContext(2, timestampBase + samplesPerFrame));

        for (ushort i = 4; i <= 30; i++)
        {
            handler.Handle(CreateContext(i, timestampBase + ((uint)(i - 1) * samplesPerFrame)));
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 10000, samplesPerFrame, 30);
    }

    [TestMethod]
    public void TestAllValidFrameSizesSequential()
    {
        // Send groups of packets at each valid Opus frame size one after another.
        // 5 packets per frame size, changing immediately between sizes.
        uint[] frameSizes = [120, 240, 480, 960, 1920, 2880, 5760];
        int packetsPerSize = 5;
        int totalPackets = frameSizes.Length * packetsPerSize; // 35

        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] expectedTimestamps = new uint[totalPackets];
        int packetIndex = 0;

        for (int sizeIdx = 0; sizeIdx < frameSizes.Length; sizeIdx++)
        {
            var frameSize = frameSizes[sizeIdx];
            for (int j = 0; j < packetsPerSize; j++)
            {
                expectedTimestamps[packetIndex] = timestamp;
                ushort seq = (ushort)(packetIndex + 1);
                handler.Handle(CreateContext(seq, timestamp));
                timestamp += frameSize;
                packetIndex++;
            }
        }

        Assert.HasCount(totalPackets, receivedPackets, $"All {totalPackets} packets should be delivered.");
        for (int i = 0; i < totalPackets; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} was marked as missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            Assert.AreEqual(expectedTimestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }
    }

    [TestMethod]
    public async Task TestFrameSizeChangeFromLargeToSmallWithBurstLoss()
    {
        // Large frames (60ms = 2880 samples) transition to small frames (2.5ms = 120 samples)
        // with a burst loss spanning the transition.
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        uint[] timestamps = new uint[40];
        for (int i = 0; i < 40; i++)
        {
            timestamps[i] = timestamp;
            timestamp += (uint)(i < 10 ? 2880 : 120);
        }

        // Send 1-10 at 60ms
        for (ushort i = 1; i <= 10; i++)
        {
            handler.Handle(CreateContext(i, timestamps[i - 1], frameSamples: 2880));
        }

        // Skip 11-15 (burst loss at transition)

        // Send 16-40 at 2.5ms
        for (ushort i = 16; i <= 40; i++)
        {
            handler.Handle(CreateContext(i, timestamps[i - 1], frameSamples: 120));
        }

        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // The 5 lost packets (11-15) are merged into a single lost event.
        // Total lost samples: timestamps[15] - (timestamps[9] + 2880) = 39400 - 38800 = 600
        Assert.HasCount(36, receivedPackets, "Should have 36 entries (35 delivered + 1 merged lost).");

        // Verify first 10 delivered packets
        for (int i = 0; i < 10; i++)
        {
            var p = receivedPackets[i];
            var expectedSeq = (ushort)(i + 1);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {i}.");
            Assert.AreEqual(timestamps[i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }

        // Verify merged lost event for packets 11-15
        {
            var p = receivedPackets[10];
            Assert.IsTrue(p.Missed, "Packets 11-15 should appear as a merged lost event.");
            Assert.AreEqual(timestamps[10], p.Timestamp, "Lost event should start at timestamp of first missing packet.");
            Assert.AreEqual(600, p.SamplesPerChannel, "Merged lost event should cover 600 samples.");
            Assert.IsTrue(p.DecodeFec, "Merged lost event should have DecodeFec set (FEC from next delivered packet).");
        }

        // Verify remaining delivered packets 16-40
        for (int i = 0; i < 25; i++)
        {
            var p = receivedPackets[11 + i];
            var expectedSeq = (ushort)(16 + i);
            Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq} at position {11 + i}.");
            Assert.AreEqual(timestamps[15 + i], p.Timestamp, $"Timestamp mismatch at seq {expectedSeq}.");
        }
    }

    [TestMethod]
    public async Task Test()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 100,
            StartupDuration = 100,
            ResynchronizationDuration = 1000,
        });

        handler.VoiceReceive += args =>
        {
            Debug.WriteLine($"Received packet: Seq={args.SequenceNumber}, TS={args.Timestamp}, Lost={args.IsLost}, Samples={(args.IsLost ? args.AsLost().SamplesPerChannel : -1)}, FEC={args.IsLost && args.AsLost().DecodeFec}");
        };

        handler.Handle(CreateContext(unchecked((ushort)(1-10)), 10000 - (10 * SamplesPerPacket)));
        uint timestamp = 10000;
        ushort seq = 1;

        for (int i = 1; i <= 100; i++)
        {
            timestamp += SamplesPerPacket;

            if (i % 2 > 0)
                continue;

            handler.Handle(CreateContext((ushort)(seq + i), timestamp));

            //handler.Handle(CreatePacket((ushort)(seq + i - 1), timestamp - SamplesPerPacket));
        }

        Debug.WriteLine("End");

        await Task.Delay(2000, context.CancellationToken).ConfigureAwait(false);
    }
}
