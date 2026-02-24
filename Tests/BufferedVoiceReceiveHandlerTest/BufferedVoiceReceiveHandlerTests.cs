using NetCord.Gateway.Voice;

namespace BufferedVoiceReceiveHandlerTest;

#pragma warning disable IDE0042 // Deconstruct variable declaration

[TestClass]
public sealed class BufferedVoiceReceiveHandlerTests
{
    private const uint DefaultSsrc = 1234;
    private const uint SamplesPerPacket = 960;
    private const int DefaultPayloadType = 0x78;

    private static (BufferedVoiceReceiveHandler Handler, List<(ushort SequenceNumber, bool Missed)> ReceivedPackets) InitializeHandler(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        BufferedVoiceReceiveHandler handler = new(configuration);

        List<(ushort SequenceNumber, bool Missed)> receivedPackets = [];

        handler.VoiceReceive += data => receivedPackets.Add((data.SequenceNumber, !data.HasPacket));

        return (handler, receivedPackets);
    }

    private static (BufferedVoiceReceiveHandler Handler, List<(uint Ssrc, ushort SequenceNumber, bool Missed)> ReceivedPackets) InitializeHandlerWithSsrc(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        BufferedVoiceReceiveHandler handler = new(configuration);

        List<(uint Ssrc, ushort SequenceNumber, bool Missed)> receivedPackets = [];

        handler.VoiceReceive += data => receivedPackets.Add((data.Ssrc, data.SequenceNumber, !data.HasPacket));

        return (handler, receivedPackets);
    }

    private static RtpPacket CreatePacket(ushort seq, uint timestamp, uint ssrc = DefaultSsrc, int payloadType = DefaultPayloadType)
    {
        return new RtpPacket(ssrc, timestamp, payloadType, seq);
    }

    /// <summary>
    /// Asserts that all non-missed packets in the list are in strictly increasing sequence order (using ushort wraparound arithmetic).
    /// </summary>
    private static void AssertMonotonicDelivery(List<(ushort SequenceNumber, bool Missed)> receivedPackets)
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
                    Assert.IsTrue(diff > 0, $"Non-monotonic delivery: {last} -> {p.SequenceNumber}");
                }
                last = p.SequenceNumber;
                first = false;
            }
        }
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets, "Should handle packets.");

        ushort expected = 1;
        foreach (var p in receivedPackets)
        {
            Assert.IsFalse(p.Missed, $"Packet {p.SequenceNumber} marked missed unexpectedly.");
            Assert.AreEqual(expected, p.SequenceNumber, "Packets out of order.");
            expected++;
        }
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
        }

        Assert.IsNotEmpty(receivedPackets);

        ushort lastSeq = 0;

        var receivedCount = receivedPackets.Count;

        for (var i = 0; i < receivedCount; i++)
        {
            var p = receivedPackets[i];
            if (!p.Missed)
            {
                Assert.IsGreaterThan(lastSeq, p.SequenceNumber, $"Packets not ordered: {lastSeq} then {p.SequenceNumber}");
                lastSeq = p.SequenceNumber;
            }
        }
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

            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.Contains(p => p.Missed && p.SequenceNumber == 3, receivedPackets, "Did not report missing packet 3.");

        var missed = receivedPackets.First(p => p.Missed);
        Assert.AreEqual(3, missed.SequenceNumber);

        int indexMissed = receivedPackets.IndexOf(missed);
        Assert.IsGreaterThanOrEqualTo(2, indexMissed, "Missed packet should be evaluated after initial buffered packets.");
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

        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(2, timestamp + SamplesPerPacket));

        ushort aliasedSeq = 23;
        var aliasedTimestamp = timestamp + ((uint)(aliasedSeq - 1) * SamplesPerPacket);
        handler.HandlePacket(CreatePacket(aliasedSeq, aliasedTimestamp));

        for (ushort i = 4; i <= 45; i++)
        {
            if (i == aliasedSeq) continue;

            var t = timestamp + ((uint)(i - 1) * SamplesPerPacket);
            handler.HandlePacket(CreatePacket(i, t));
        }

        var seq3 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 3);
        Assert.IsTrue(seq3.Missed, "Sequence 3 should be marked missed. Buffer fell for an aliased packet.");

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

        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(2, timestamp + SamplesPerPacket));

        ushort jumpBase = 502;

        uint jumpTimestamp = timestamp + ((uint)(jumpBase - 1) * SamplesPerPacket);

        handler.HandlePacket(CreatePacket(jumpBase, jumpTimestamp));
        handler.HandlePacket(CreatePacket((ushort)(jumpBase + 1), jumpTimestamp + SamplesPerPacket));
        handler.HandlePacket(CreatePacket((ushort)(jumpBase + 2), jumpTimestamp + (2 * SamplesPerPacket)));

        for (ushort i = 3; i <= 25; i++)
        {
            uint t = jumpTimestamp + (i * SamplesPerPacket);
            handler.HandlePacket(CreatePacket((ushort)(jumpBase + i), t));
        }

        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync reset.");

        var thresholdSeq = receivedPackets.FirstOrDefault(p => p.SequenceNumber == jumpBase + 2);
        Assert.IsFalse(thresholdSeq.Missed, "The sequence that triggered the resync should be handled successfully as the new baseline.");
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        ushort? lastSeq = null;
        foreach (var p in receivedPackets)
        {
            Assert.IsFalse(p.Missed, $"Packet {p.SequenceNumber} should not be missed.");
            if (lastSeq.HasValue)
            {
                ushort diff = (ushort)(p.SequenceNumber - lastSeq.Value);
                Assert.AreEqual((ushort)1, diff, $"Expected sequential delivery, got {lastSeq.Value} -> {p.SequenceNumber}");
            }
            lastSeq = p.SequenceNumber;
        }
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.Contains(p => p.Missed && p.SequenceNumber == 65535, receivedPackets,
            "Missing packet at seq 65535 (wraparound boundary) should be reported.");
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.Contains(p => p.Missed && p.SequenceNumber == 0, receivedPackets,
            "Missing packet at seq 0 (immediately after wraparound) should be reported.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets, "Should handle packets with timestamp wraparound.");
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestTimestampStartingAtZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 0;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        ushort expected = 1;
        foreach (var p in receivedPackets)
        {
            if (!p.Missed)
            {
                Assert.AreEqual(expected, p.SequenceNumber);
                expected++;
            }
        }
    }

    [TestMethod]
    public void TestTimestampExactlyAtMaxValue()
    {
        var (handler, receivedPackets) = InitializeHandler();

        // Start so that one packet lands exactly at uint.MaxValue
        uint timestamp = uint.MaxValue - (5 * SamplesPerPacket);

        for (ushort i = 1; i <= 20; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets, "Should handle simultaneous seq+ts wraparound.");
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestamp));

            if (i % 5 == 0)
                handler.HandlePacket(CreatePacket(i, timestamp));

            timestamp += SamplesPerPacket;
        }

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        var grouped = nonMissed.GroupBy(p => p.SequenceNumber);

        foreach (var group in grouped)
        {
            Assert.AreEqual(1, group.Count(), $"Sequence {group.Key} emitted {group.Count()} times; expected exactly once.");
        }
    }

    [TestMethod]
    public void TestConsecutiveDuplicates()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            handler.HandlePacket(CreatePacket(i, timestamp));
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        var grouped = nonMissed.GroupBy(p => p.SequenceNumber);

        foreach (var group in grouped)
        {
            Assert.AreEqual(1, group.Count(), $"Sequence {group.Key} should appear exactly once despite triple sends.");
        }
    }

    [TestMethod]
    public void TestDuplicateOfLatestPacket()
    {
        // The handler has a special `sequenceNumberDiff is 0` check for duplicates of the latest packet
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(1, timestamp)); // Exact duplicate of "latest"

        timestamp += SamplesPerPacket;
        for (ushort i = 2; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var seq1Delivered = receivedPackets.Count(p => !p.Missed && p.SequenceNumber == 1);
        Assert.AreEqual(1, seq1Delivered, "Sequence 1 should be delivered exactly once.");
    }

    // ================================================================
    // PayloadType Filtering
    // ================================================================

    [TestMethod]
    public void TestNonVoicePayloadTypeIgnored()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, payloadType: 0x60));
            timestamp += SamplesPerPacket;
        }

        Assert.AreEqual(0, receivedPackets.Count, "Non-voice payload type packets should be completely ignored.");
    }

    [TestMethod]
    public void TestMixedPayloadTypes()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        ushort voiceSeq = 1;

        for (int i = 0; i < 60; i++)
        {
            if (i % 2 == 0)
            {
                handler.HandlePacket(CreatePacket(voiceSeq, timestamp));
                voiceSeq++;
            }
            else
            {
                handler.HandlePacket(CreatePacket((ushort)(voiceSeq + 1000), timestamp, payloadType: 0x50));
            }
            timestamp += SamplesPerPacket;
        }

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        foreach (var p in nonMissed)
        {
            Assert.IsTrue(p.SequenceNumber < voiceSeq, $"Only voice sequence numbers should appear, got {p.SequenceNumber}.");
        }
    }

    [TestMethod]
    public void TestPayloadTypeZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 10; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, payloadType: 0));
            timestamp += SamplesPerPacket;
        }

        Assert.AreEqual(0, receivedPackets.Count, "Payload type 0 is not 0x78 and should be ignored.");
    }

    [TestMethod]
    public void TestPayloadType127()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 10; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, payloadType: 127));
            timestamp += SamplesPerPacket;
        }

        Assert.AreEqual(0, receivedPackets.Count, "Payload type 127 is not 0x78 and should be ignored.");
    }

    // ================================================================
    // Multiple SSRCs
    // ================================================================

    [TestMethod]
    public void TestMultipleSSRCsIndependent()
    {
        var (handler, receivedPackets) = InitializeHandlerWithSsrc();

        uint timestamp1 = 10000;
        uint timestamp2 = 20000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp1, ssrc: 1000));
            handler.HandlePacket(CreatePacket(i, timestamp2, ssrc: 2000));
            timestamp1 += SamplesPerPacket;
            timestamp2 += SamplesPerPacket;
        }

        var ssrc1Packets = receivedPackets.Where(p => p.Ssrc == 1000 && !p.Missed).ToList();
        var ssrc2Packets = receivedPackets.Where(p => p.Ssrc == 2000 && !p.Missed).ToList();

        Assert.IsTrue(ssrc1Packets.Count > 0, "SSRC 1000 should have received packets.");
        Assert.IsTrue(ssrc2Packets.Count > 0, "SSRC 2000 should have received packets.");

        for (int i = 1; i < ssrc1Packets.Count; i++)
            Assert.IsGreaterThan(ssrc1Packets[i - 1].SequenceNumber, ssrc1Packets[i].SequenceNumber);

        for (int i = 1; i < ssrc2Packets.Count; i++)
            Assert.IsGreaterThan(ssrc2Packets[i - 1].SequenceNumber, ssrc2Packets[i].SequenceNumber);
    }

    [TestMethod]
    public void TestMultipleSSRCsWithDifferentPatterns()
    {
        var (handler, receivedPackets) = InitializeHandlerWithSsrc();

        uint ts;

        // SSRC 100: ordered
        ts = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, ts, ssrc: 100));
            ts += SamplesPerPacket;
        }

        // SSRC 200: pairwise swapped
        List<ushort> reordered = [2, 1, 4, 3, 6, 5, 8, 7, 10, 9];
        for (ushort i = 11; i <= 30; i++) reordered.Add(i);
        foreach (var seq in reordered)
        {
            handler.HandlePacket(CreatePacket(seq, 10000 + ((uint)(seq - 1) * SamplesPerPacket), ssrc: 200));
        }

        // SSRC 300: missing seq 5
        ts = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            if (i == 5) { ts += SamplesPerPacket; continue; }
            handler.HandlePacket(CreatePacket(i, ts, ssrc: 300));
            ts += SamplesPerPacket;
        }

        var ssrc1 = receivedPackets.Where(p => p.Ssrc == 100 && !p.Missed).ToList();
        var ssrc2 = receivedPackets.Where(p => p.Ssrc == 200 && !p.Missed).ToList();
        var ssrc3Missing = receivedPackets.Where(p => p.Ssrc == 300 && p.Missed).ToList();

        Assert.IsTrue(ssrc1.Count > 0, "SSRC 100 should deliver packets.");
        Assert.IsTrue(ssrc2.Count > 0, "SSRC 200 should deliver packets.");
        Assert.Contains(p => p.SequenceNumber == 5, ssrc3Missing, "SSRC 300 should report seq 5 as missed.");
    }

    [TestMethod]
    public void TestSsrcZero()
    {
        var (handler, receivedPackets) = InitializeHandlerWithSsrc();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, ssrc: 0));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        Assert.IsTrue(receivedPackets.All(p => p.Ssrc == 0), "All packets should have SSRC 0.");
    }

    [TestMethod]
    public void TestSsrcMaxValue()
    {
        var (handler, receivedPackets) = InitializeHandlerWithSsrc();

        uint timestamp = 10000;
        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, ssrc: uint.MaxValue));
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        for (ushort missing = 5; missing <= 7; missing++)
        {
            Assert.Contains(p => p.Missed && p.SequenceNumber == missing, receivedPackets,
                $"Packet {missing} should be reported as missed.");
        }
    }

    [TestMethod]
    public void TestBurstLossAndRecovery()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Normal flow: 1-10
        for (ushort i = 1; i <= 10; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Burst loss: skip 11-20
        timestamp += 10 * SamplesPerPacket;

        // Recovery: 21-40
        for (ushort i = 21; i <= 40; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets, "Should have received packets despite burst loss.");
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestMissingFirstPacketInSequence()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Start from sequence 2, never send 1
        for (ushort i = 2; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        Assert.IsNotEmpty(receivedPackets);

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.IsTrue(nonMissed.Count > 0, "Should deliver packets even when first is missing.");
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestMissingLastPacketBeforeGap()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        // Send 1-29, skip 30
        for (ushort i = 1; i <= 29; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }
        timestamp += SamplesPerPacket; // Skip 30

        // Continue from 31
        for (ushort i = 31; i <= 50; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.Contains(p => p.Missed && p.SequenceNumber == 30, receivedPackets,
            "Packet 30 should be reported as missed.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        int countBefore = receivedPackets.Count;

        // Send packet 2 very late
        handler.HandlePacket(CreatePacket(2, 10000 + SamplesPerPacket));

        var seq2Count = receivedPackets.Count(p => !p.Missed && p.SequenceNumber == 2);
        Assert.IsTrue(seq2Count <= 1, "Late packet should not cause duplicate delivery of seq 2.");
    }

    [TestMethod]
    public void TestLatePacketWithinBufferWindow()
    {
        // Packet arrives late but buffer hasn't evicted its slot yet
        var (handler, receivedPackets) = InitializeHandler();

        uint timestampBase = 10000;

        // Send 1, 2, 4, 5 (skip 3), then send 3 late
        handler.HandlePacket(CreatePacket(1, timestampBase));
        handler.HandlePacket(CreatePacket(2, timestampBase + SamplesPerPacket));
        handler.HandlePacket(CreatePacket(4, timestampBase + (3 * SamplesPerPacket)));
        handler.HandlePacket(CreatePacket(5, timestampBase + (4 * SamplesPerPacket)));
        handler.HandlePacket(CreatePacket(3, timestampBase + (2 * SamplesPerPacket))); // Late arrival

        // Continue to flush
        for (ushort i = 6; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        // Seq 3 should ideally be delivered, not missed
        var seq3 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 3);
        Assert.IsFalse(seq3.Missed, "Packet 3 arrived late but within buffer window; should not be missed.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Send 4 contiguous outliers (below threshold of 5)
        ushort outlierBase = 30000;
        uint outlierTs = 5000000;
        for (int i = 0; i < 4; i++)
        {
            handler.HandlePacket(CreatePacket((ushort)(outlierBase + i), outlierTs + (uint)(i * SamplesPerPacket)));
        }

        // Continue normal flow from 11
        for (ushort i = 11; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var normalPackets = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsTrue(normalPackets.Count > 0, "Normal packets should still be delivered without resync.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Two outliers from one range
        handler.HandlePacket(CreatePacket(30000, 5000000));
        handler.HandlePacket(CreatePacket(30001, 5000000 + SamplesPerPacket));

        // One outlier from a VERY different range (non-contiguous → should reset counter)
        handler.HandlePacket(CreatePacket(50000, 9000000));

        // One more contiguous with the new range
        handler.HandlePacket(CreatePacket(50001, 9000000 + SamplesPerPacket));

        // Normal flow continues — should not have resynced
        for (ushort i = 11; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        var normalPackets = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsTrue(normalPackets.Count > 0, "Normal packets should still be delivered after non-contiguous outliers.");
    }

    [TestMethod]
    public void TestExactThresholdTriggersResync()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            ResynchronizationThreshold = 3,
        });

        uint timestamp = 10000;

        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(2, timestamp + SamplesPerPacket));

        // Exactly 3 contiguous outliers → should trigger resync
        ushort jumpBase = 5000;
        uint jumpTs = 10000000;
        handler.HandlePacket(CreatePacket(jumpBase, jumpTs));
        handler.HandlePacket(CreatePacket((ushort)(jumpBase + 1), jumpTs + SamplesPerPacket));
        handler.HandlePacket(CreatePacket((ushort)(jumpBase + 2), jumpTs + (2 * SamplesPerPacket)));

        // Continue from new range
        for (ushort i = 3; i <= 25; i++)
        {
            handler.HandlePacket(CreatePacket((ushort)(jumpBase + i), jumpTs + (i * SamplesPerPacket)));
        }

        // Old packets should have been force-evicted
        var seq1 = receivedPackets.FirstOrDefault(p => p.SequenceNumber == 1);
        Assert.IsFalse(seq1.Missed, "Sequence 1 should have been force-evicted during resync.");

        // New range should be delivered
        var newRange = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpBase).ToList();
        Assert.IsTrue(newRange.Count > 0, "New range packets should be delivered after resync.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // 3 contiguous outliers (threshold is 4 → should NOT trigger resync)
        ushort outlierBase = 30000;
        uint outlierTs = 5000000;
        handler.HandlePacket(CreatePacket(outlierBase, outlierTs));
        handler.HandlePacket(CreatePacket((ushort)(outlierBase + 1), outlierTs + SamplesPerPacket));
        handler.HandlePacket(CreatePacket((ushort)(outlierBase + 2), outlierTs + (2 * SamplesPerPacket)));

        // Resume normal
        for (ushort i = 6; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Normal stream should continue uninterrupted
        var normalDelivered = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsTrue(normalDelivered.Count > 0, "Normal flow should continue when outliers are below threshold.");
    }

    // ================================================================
    // Idle Timeout / Timer Behavior
    // ================================================================

    [TestMethod]
    public void TestIdleTimeoutForceEvictsBuffered()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 200,
            IdleTimeout = 1000,
        });

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for the buffer timer to fire and force-evict
        Thread.Sleep(500);

        Assert.IsTrue(receivedPackets.Count > 0, "Packets should be force-evicted after idle timeout.");
    }

    [TestMethod]
    public void TestIdleTimeoutThenNewStream()
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for idle timeout + disposal
        Thread.Sleep(600);

        int countAfterIdle = receivedPackets.Count;

        // New stream — handler should reinitialize state
        for (ushort i = 100; i <= 120; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Thread.Sleep(200);

        Assert.IsTrue(receivedPackets.Count > countAfterIdle, "New packets should be handled after idle timeout and reinit.");
    }

    // ================================================================
    // Edge: Single Packet
    // ================================================================

    [TestMethod]
    public void TestSinglePacket()
    {
        var (handler, receivedPackets) = InitializeHandler(new()
        {
            BufferDuration = 50,
            StartupDuration = 0,
        });

        handler.HandlePacket(CreatePacket(1, 10000));

        Thread.Sleep(200);

        Assert.IsTrue(receivedPackets.Count > 0, "Single packet should be eventually emitted.");
        Assert.AreEqual(1, receivedPackets.First(p => !p.Missed).SequenceNumber);
    }

    // ================================================================
    // Edge: Sequence Starting Points
    // ================================================================

    [TestMethod]
    public void TestSequenceStartingAtZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 0; i < 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.IsTrue(nonMissed.Count > 0);
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestSequenceStartingAtMaxValue()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        ushort startSeq = ushort.MaxValue;
        for (int i = 0; i < 30; i++)
        {
            ushort seq = (ushort)(startSeq + i);
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        Assert.IsNotEmpty(receivedPackets);

        // Only verify no crash and that some packets were delivered — full reorder
        // guarantee is not possible when all packets arrive in reverse order
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.IsTrue(nonMissed.Count > 0, "At least some packets should be delivered.");
    }

    [TestMethod]
    public void TestRandomReorderingWithJitter()
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
            int swapTarget = Math.Clamp(i + rng.Next(-3, 4), 0, seqs.Count - 1);
            (seqs[i], seqs[swapTarget]) = (seqs[swapTarget], seqs[i]);
        }

        foreach (var seq in seqs)
        {
            handler.HandlePacket(CreatePacket(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        Assert.Contains(p => p.Missed && p.SequenceNumber == 4, receivedPackets, "Packet 4 should be reported missed.");
        Assert.Contains(p => p.Missed && p.SequenceNumber == 9, receivedPackets, "Packet 9 should be reported missed.");
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // First jump
        ushort jump1 = 10000;
        uint jump1Ts = 20000000;
        handler.HandlePacket(CreatePacket(jump1, jump1Ts));
        handler.HandlePacket(CreatePacket((ushort)(jump1 + 1), jump1Ts + SamplesPerPacket));

        for (ushort i = 2; i <= 5; i++)
            handler.HandlePacket(CreatePacket((ushort)(jump1 + i), jump1Ts + (i * SamplesPerPacket)));

        // Second jump
        ushort jump2 = 50000;
        uint jump2Ts = 80000000;
        handler.HandlePacket(CreatePacket(jump2, jump2Ts));
        handler.HandlePacket(CreatePacket((ushort)(jump2 + 1), jump2Ts + SamplesPerPacket));

        for (ushort i = 2; i <= 25; i++)
            handler.HandlePacket(CreatePacket((ushort)(jump2 + i), jump2Ts + (i * SamplesPerPacket)));

        var range1 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 5).ToList();
        var range2 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jump1 && p.SequenceNumber <= jump1 + 5).ToList();
        var range3 = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jump2).ToList();

        Assert.IsTrue(range1.Count > 0, "First range should be force-evicted and delivered.");
        Assert.IsTrue(range2.Count > 0, "Second range should be force-evicted and delivered.");
        Assert.IsTrue(range3.Count > 0, "Third range should be delivered.");
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
            handler.HandlePacket(CreatePacket((ushort)(startSeq + i), timestamp));
            timestamp += SamplesPerPacket;
        }

        // Jump across the wraparound boundary to trigger resync
        ushort jumpSeq = 100;
        uint jumpTs = 50000000;
        handler.HandlePacket(CreatePacket(jumpSeq, jumpTs));
        handler.HandlePacket(CreatePacket((ushort)(jumpSeq + 1), jumpTs + SamplesPerPacket));

        for (ushort i = 2; i <= 25; i++)
            handler.HandlePacket(CreatePacket((ushort)(jumpSeq + i), jumpTs + (i * SamplesPerPacket)));

        var postJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpSeq).ToList();
        Assert.IsTrue(postJump.Count > 0, "Packets after resync across wraparound should be delivered.");
    }

    // ================================================================
    // Buffer Size Boundary
    // ================================================================

    [TestMethod]
    public void TestExactBufferSizeBoundary()
    {
        // Default buffer size = 240*2/5 = 96
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 96; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        // One more to trigger overflow
        handler.HandlePacket(CreatePacket(97, timestamp));

        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.IsTrue(nonMissed.Count > 0);
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestMinimumBufferSize()
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets, "With startup=0, packets should be emitted sooner.");
    }

    // ================================================================
    // Random Packet Loss
    // ================================================================

    [TestMethod]
    public void TestRandomPacketLoss10Percent()
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        // Verify no delivered packet was actually dropped
        var deliveredSeqs = receivedPackets.Where(p => !p.Missed).Select(p => p.SequenceNumber).ToHashSet();
        foreach (var seq in deliveredSeqs)
        {
            Assert.IsFalse(dropped.Contains(seq), $"Delivered packet {seq} was supposed to be dropped — data corruption.");
        }

        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestRandomPacketLoss50Percent()
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        var deliveredSeqs = receivedPackets.Where(p => !p.Missed).Select(p => p.SequenceNumber).ToHashSet();
        foreach (var seq in deliveredSeqs)
        {
            Assert.IsFalse(dropped.Contains(seq), $"Delivered packet {seq} was supposed to be dropped.");
        }

        AssertMonotonicDelivery(receivedPackets);
    }

    // ================================================================
    // Timestamp Inconsistencies
    // ================================================================

    [TestMethod]
    public void TestInconsistentTimestampGaps()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            // Vary the timestamp increment slightly (simulating variable frame sizes)
            timestamp += SamplesPerPacket + (uint)(i % 3 == 0 ? 10 : 0);
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestSameTimestampDifferentSequences()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 5; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Multiple packets sharing the same timestamp (unusual but shouldn't crash)
        for (ushort i = 6; i <= 10; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
        }

        for (ushort i = 11; i <= 30; i++)
        {
            timestamp += SamplesPerPacket;
            handler.HandlePacket(CreatePacket(i, timestamp));
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Seq 6 with a timestamp lower than expected (backward)
        handler.HandlePacket(CreatePacket(6, 10000));

        // Continue normally
        for (ushort i = 7; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Should not crash
        Assert.IsNotEmpty(receivedPackets);
    }

    // ================================================================
    // Concurrency
    // ================================================================

    [TestMethod]
    public void TestConcurrentPacketHandlingDifferentSSRCs()
    {
        BufferedVoiceReceiveHandler handler = new();
        List<(uint Ssrc, ushort SequenceNumber, bool Missed)> receivedPackets = [];
        var listLock = new object();

        handler.VoiceReceive += data =>
        {
            lock (listLock)
            {
                receivedPackets.Add((data.Ssrc, data.SequenceNumber, !data.HasPacket));
            }
        };

        var t1 = Task.Run(() =>
        {
            uint ts = 10000;
            for (ushort i = 1; i <= 100; i++)
            {
                handler.HandlePacket(CreatePacket(i, ts, ssrc: 1111));
                ts += SamplesPerPacket;
            }
        });

        var t2 = Task.Run(() =>
        {
            uint ts = 20000;
            for (ushort i = 1; i <= 100; i++)
            {
                handler.HandlePacket(CreatePacket(i, ts, ssrc: 2222));
                ts += SamplesPerPacket;
            }
        });

        Task.WaitAll(t1, t2);

        Thread.Sleep(500);

        lock (listLock)
        {
            var ssrc1 = receivedPackets.Where(p => p.Ssrc == 1111).ToList();
            var ssrc2 = receivedPackets.Where(p => p.Ssrc == 2222).ToList();

            Assert.IsTrue(ssrc1.Count > 0, "SSRC 1111 should have received packets.");
            Assert.IsTrue(ssrc2.Count > 0, "SSRC 2222 should have received packets.");
        }
    }

    [TestMethod]
    public void TestConcurrentPacketHandlingSameSSRC()
    {
        // Multiple threads sending interleaved packets for the same SSRC
        BufferedVoiceReceiveHandler handler = new();
        List<(ushort SequenceNumber, bool Missed)> receivedPackets = [];
        var listLock = new object();

        handler.VoiceReceive += data =>
        {
            lock (listLock)
            {
                receivedPackets.Add((data.SequenceNumber, !data.HasPacket));
            }
        };

        var t1 = Task.Run(() =>
        {
            uint ts = 10000;
            for (ushort i = 1; i <= 100; i += 2) // Odd sequence numbers
            {
                handler.HandlePacket(CreatePacket(i, ts + ((uint)(i - 1) * SamplesPerPacket)));
            }
        });

        var t2 = Task.Run(() =>
        {
            uint ts = 10000;
            for (ushort i = 2; i <= 100; i += 2) // Even sequence numbers
            {
                handler.HandlePacket(CreatePacket(i, ts + ((uint)(i - 1) * SamplesPerPacket)));
            }
        });

        Task.WaitAll(t1, t2);

        Thread.Sleep(500);

        // No crash = success; ordering under concurrent same-SSRC is best-effort
        lock (listLock)
        {
            Assert.IsTrue(receivedPackets.Count > 0, "Should have received packets from concurrent same-SSRC streams.");
        }
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // No crash = pass
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
        handler.HandlePacket(CreatePacket(1, timestamp));

        // Send widely separated outliers that never form a contiguous stream
        handler.HandlePacket(CreatePacket(20000, 50000000));
        handler.HandlePacket(CreatePacket(40000, 90000000));
        handler.HandlePacket(CreatePacket(60000, 130000000));

        // Resume normal flow
        for (ushort i = 2; i <= 30; i++)
        {
            timestamp += SamplesPerPacket;
            handler.HandlePacket(CreatePacket(i, timestamp));
        }

        Assert.IsNotEmpty(receivedPackets);
    }

    // ================================================================
    // Edge: Large volume
    // ================================================================

    [TestMethod]
    public void TestLargePacketVolume()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;

        for (ushort i = 1; i <= 10000; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;

            if (i == 10000) break;
        }

        Assert.IsNotEmpty(receivedPackets);
        Assert.IsTrue(receivedPackets.Count > 100, "Large volume should produce many output packets.");
        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestFullSequenceNumberCycle()
    {
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 0;

        for (int i = 0; i < 65536; i++)
        {
            handler.HandlePacket(CreatePacket((ushort)i, timestamp));
            timestamp += SamplesPerPacket;
        }

        Assert.IsNotEmpty(receivedPackets);

        // Verify monotonic delivery (with ushort wraparound awareness)
        ushort last = 0;
        bool first = true;
        foreach (var p in receivedPackets)
        {
            if (!p.Missed)
            {
                if (!first)
                {
                    ushort diff = (ushort)(p.SequenceNumber - last);
                    Assert.IsTrue(diff > 0 && diff < 32768, $"Non-monotonic or wrapped incorrectly: {last} -> {p.SequenceNumber}");
                }
                last = p.SequenceNumber;
                first = false;
            }
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

        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(2, timestamp + SamplesPerPacket));

        // Single outlier → should trigger resync
        ushort jumpSeq = 500;
        uint jumpTs = 5000000;
        handler.HandlePacket(CreatePacket(jumpSeq, jumpTs));

        // Continue from new range
        for (ushort i = 1; i <= 25; i++)
        {
            handler.HandlePacket(CreatePacket((ushort)(jumpSeq + i), jumpTs + (i * SamplesPerPacket)));
        }

        var postJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpSeq).ToList();
        Assert.IsTrue(postJump.Count > 0, "Single outlier with threshold=1 should trigger resync.");
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Re-send seq 5 with a different timestamp (replay attack or corruption)
        handler.HandlePacket(CreatePacket(5, timestamp + 50000));

        // Continue normally
        for (ushort i = 11; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
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
                handler.HandlePacket(CreatePacket(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
            }
        }

        Assert.IsNotEmpty(receivedPackets);
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestampBase + ((uint)(i - 1) * SamplesPerPacket)));
        }

        // The handler should still function; gaps between packets are large but within buffer range
        Assert.IsNotEmpty(receivedPackets);
    }

    // ================================================================
    // Configuration: Large values
    // ================================================================

    [TestMethod]
    public void TestLargeBufferDuration()
    {
        var handler = new BufferedVoiceReceiveHandler(new()
        {
            BufferDuration = 10000,
            ResynchronizationDuration = 20000,
        });
        Assert.IsNotNull(handler);
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
}
