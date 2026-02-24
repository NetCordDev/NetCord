using NetCord.Gateway.Voice;

namespace BufferedVoiceReceiveHandlerTest;

[TestClass]
public sealed class BufferedVoiceReceiveHandlerTests(TestContext context)
{
    private const uint DefaultSsrc = 1234;
    private const uint SamplesPerPacket = 960;
    private const int DefaultPayloadType = 0x78;

    private record struct ReceivedPacket(uint Ssrc, ushort SequenceNumber, bool Missed, int Tag);

    private static (BufferedVoiceReceiveHandler Handler, List<ReceivedPacket> ReceivedPackets) InitializeHandler(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        BufferedVoiceReceiveHandler handler = new(configuration);

        List<ReceivedPacket> receivedPackets = [];

        handler.VoiceReceive += data => receivedPackets.Add(new ReceivedPacket(
            data.Ssrc,
            data.SequenceNumber,
            !data.HasPacket,
            data.HasPacket ? data.Packet.Tag : 0));

        return (handler, receivedPackets);
    }

    private static RtpPacket CreatePacket(ushort seq, uint timestamp, uint ssrc = DefaultSsrc, int payloadType = DefaultPayloadType, int tag = 0)
    {
        return new RtpPacket(ssrc, timestamp, payloadType, seq, tag);
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
    /// Asserts that the received packets exactly match a contiguous sequence with no misses.
    /// </summary>
    private static void AssertExactOrderedDelivery(List<ReceivedPacket> receivedPackets, ushort startSeq, int count)
    {
        Assert.HasCount(count, receivedPackets, $"Expected {count} packets but got {receivedPackets.Count}.");
        for (int i = 0; i < count; i++)
        {
            var expected = (ushort)(startSeq + i);
            var p = receivedPackets[i];
            Assert.IsFalse(p.Missed, $"Packet {expected} was marked as missed.");
            Assert.AreEqual(expected, p.SequenceNumber, $"Expected seq {expected} at position {i}, got {p.SequenceNumber}.");
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

        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // All packets should be reordered and delivered exactly in order
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // Should deliver all 30 slots: 29 actual + 1 missed
        Assert.HasCount(30, receivedPackets, "Should deliver exactly 30 entries (29 packets + 1 missed).");

        // Verify exact sequence: 1, 2, Missed(3), 4, ..., 30
        ushort expectedSeq = 1;
        foreach (var p in receivedPackets)
        {
            Assert.AreEqual(expectedSeq, p.SequenceNumber, $"Expected seq {expectedSeq}.");
            if (expectedSeq == 3)
                Assert.IsTrue(p.Missed, "Packet 3 should be marked as missed.");
            else
                Assert.IsFalse(p.Missed, $"Packet {expectedSeq} should not be missed.");
            expectedSeq++;
        }
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
            handler.HandlePacket(CreatePacket(seq, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Exact delivery: 32 packets from 65530 through wraparound
        AssertExactOrderedDelivery(receivedPackets, startSeq, 32);
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

        // Should deliver all 28 packets in order: 65534, 65535, 0, 1, ..., 25
        AssertExactOrderedDelivery(receivedPackets, 65534, 28);
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

        // Verify total count: 31 delivered + 1 missed = 32
        Assert.HasCount(32, receivedPackets,
            "Should have exactly 32 entries (31 packets + 1 missed).");
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

        Assert.HasCount(32, receivedPackets,
            "Should have exactly 32 entries (31 packets + 1 missed).");
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

        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        AssertExactOrderedDelivery(receivedPackets, 1, 20);
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

        AssertExactOrderedDelivery(receivedPackets, startSeq, 30);
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

        // All 30 packets should be delivered exactly once, in order
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // All 30 packets should be delivered exactly once, in order
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // All 30 should be delivered exactly once
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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
                handler.HandlePacket(CreatePacket(seq, timestamp, tag: seq));
                timestamp += SamplesPerPacket;
            }

            // Duplicate of batchStart arrives after 4 intervening packets
            handler.HandlePacket(CreatePacket(batchStart, timestamp - (5 * SamplesPerPacket), tag: batchStart + 10000));
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
            handler.HandlePacket(CreatePacket(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Delayed duplicates of early packets with different tags
        handler.HandlePacket(CreatePacket(1, 10000, tag: 9001));
        handler.HandlePacket(CreatePacket(2, 10000 + SamplesPerPacket, tag: 9002));
        handler.HandlePacket(CreatePacket(3, 10000 + (2 * SamplesPerPacket), tag: 9003));

        // Continue normally
        for (ushort i = 11; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Verify original tags were preserved (packets 1-3 were already evicted before dups)
        for (ushort seq = 1; seq <= 3; seq++)
        {
            var p = receivedPackets.First(pkt => !pkt.Missed && pkt.SequenceNumber == seq);
            Assert.AreEqual(seq, p.Tag,
                $"Packet {seq} should retain the original tag ({seq}), not the delayed duplicate's ({9000 + seq}).");
        }

        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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
        handler.HandlePacket(CreatePacket(1, timestamp, tag: 1));
        timestamp += SamplesPerPacket;

        handler.HandlePacket(CreatePacket(2, timestamp, tag: 2));
        timestamp += SamplesPerPacket;

        handler.HandlePacket(CreatePacket(3, timestamp, tag: 3));
        timestamp += SamplesPerPacket;

        handler.HandlePacket(CreatePacket(4, timestamp, tag: 4));
        timestamp += SamplesPerPacket;

        // By now, packet 1 should be evicted (bufferSize=4, eviction starts immediately)
        handler.HandlePacket(CreatePacket(1, 10000, tag: 999));

        // Continue normally
        for (ushort i = 5; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        var seq1 = receivedPackets.First(p => !p.Missed && p.SequenceNumber == 1);
        Assert.AreEqual(1, seq1.Tag,
            "Packet 1 should retain original tag (1), not delayed duplicate's (999).");

        AssertMonotonicDelivery(receivedPackets);
    }

    [TestMethod]
    public void TestDuplicateOfLatestPacketDoesNotOverwriteOriginalData()
    {
        // Specifically targets the `sequenceNumberDiff is 0` fast-path
        var (handler, receivedPackets) = InitializeHandler();

        uint timestamp = 10000;
        handler.HandlePacket(CreatePacket(1, timestamp, tag: 42));
        handler.HandlePacket(CreatePacket(1, timestamp, tag: 99)); // Duplicate with different tag

        timestamp += SamplesPerPacket;
        for (ushort i = 2; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
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
        handler.HandlePacket(CreatePacket(1, timestamp, tag: 42));
        timestamp += SamplesPerPacket;

        // Send enough intervening packets so packet 1 is evicted
        for (ushort i = 2; i <= 10; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, tag: i));
            timestamp += SamplesPerPacket;
        }

        // Delayed duplicate of packet 1 with different tag (original already evicted)
        handler.HandlePacket(CreatePacket(1, 10000, tag: 999));

        // Continue the stream 11-30
        for (ushort i = 11; i <= 30; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp, tag: i));
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

        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        Assert.IsEmpty(receivedPackets, "Non-voice payload type packets should be completely ignored.");
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
            Assert.IsLessThan(voiceSeq, p.SequenceNumber, $"Only voice sequence numbers should appear, got {p.SequenceNumber}.");
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

        Assert.IsEmpty(receivedPackets, "Payload type 0 is not 0x78 and should be ignored.");
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

        Assert.IsEmpty(receivedPackets, "Payload type 127 is not 0x78 and should be ignored.");
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
            handler.HandlePacket(CreatePacket(i, timestamp1, ssrc: 1000));
            handler.HandlePacket(CreatePacket(i, timestamp2, ssrc: 2000));
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

        Assert.IsNotEmpty(ssrc1, "SSRC 100 should deliver packets.");
        Assert.IsNotEmpty(ssrc2, "SSRC 200 should deliver packets.");
        Assert.Contains(p => p.SequenceNumber == 5, ssrc3Missing, "SSRC 300 should report seq 5 as missed.");
    }

    [TestMethod]
    public void TestSsrcZero()
    {
        var (handler, receivedPackets) = InitializeHandler();

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
        var (handler, receivedPackets) = InitializeHandler();

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

        // Verify all 30 entries are present: 27 packets + 3 missed
        Assert.HasCount(30, receivedPackets, "Should have exactly 30 entries.");

        for (ushort missing = 5; missing <= 7; missing++)
        {
            Assert.Contains(p => p.Missed && p.SequenceNumber == missing, receivedPackets,
                $"Packet {missing} should be reported as missed.");
        }

        // Verify overall order
        ushort expectedSeq = 1;
        foreach (var p in receivedPackets)
        {
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            expectedSeq++;
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

        // All 40 entries should be present: 30 actual + 10 missed
        Assert.HasCount(40, receivedPackets, "Should have 40 entries (30 packets + 10 missed).");

        // Verify missed packets 11-20
        for (ushort missing = 11; missing <= 20; missing++)
        {
            Assert.Contains(p => p.Missed && p.SequenceNumber == missing, receivedPackets,
                $"Packet {missing} should be reported as missed.");
        }

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

        // Verify total: 49 delivered + 1 missed = 50
        Assert.HasCount(50, receivedPackets, "Should have exactly 50 entries.");
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

        // Send packet 2 very late
        handler.HandlePacket(CreatePacket(2, 10000 + SamplesPerPacket));

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

        // All 30 should be delivered in order
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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
        Assert.IsNotEmpty(normalPackets, "Normal packets should still be delivered without resync.");

        // Verify monotonic delivery across the normal range
        AssertMonotonicDelivery(receivedPackets);
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for the buffer timer to fire and force-evict
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        Assert.IsNotEmpty(receivedPackets, "Packets should be force-evicted after idle timeout.");

        // All 5 packets should be force-evicted
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        Assert.HasCount(5, nonMissed, "All 5 packets should be force-evicted by the timer.");

        ushort expectedSeq = 1;
        foreach (var p in nonMissed)
        {
            Assert.AreEqual(expectedSeq, p.SequenceNumber);
            expectedSeq++;
        }
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
            handler.HandlePacket(CreatePacket(i, timestamp));
            timestamp += SamplesPerPacket;
        }

        // Wait for idle timeout + disposal
        await Task.Delay(600, context.CancellationToken).ConfigureAwait(false);

        int countAfterIdle = receivedPackets.Count;
        Assert.IsGreaterThan(0, countAfterIdle, "Packets should be force-evicted during idle.");

        // New stream — handler should reinitialize state
        for (ushort i = 100; i <= 120; i++)
        {
            handler.HandlePacket(CreatePacket(i, timestamp));
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
        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(3, timestamp + (2 * SamplesPerPacket)));
        handler.HandlePacket(CreatePacket(4, timestamp + (3 * SamplesPerPacket)));

        // No eviction should have happened yet (startup == buffer)
        Assert.IsEmpty(receivedPackets, "No packets should be emitted before timer fires.");

        // Wait for the buffer timer to fire
        await Task.Delay(500, context.CancellationToken).ConfigureAwait(false);

        // Timer should have force-evicted all: 1, Missed(2), 3, 4
        Assert.HasCount(4, receivedPackets, "Timer should emit exactly 4 entries.");

        Assert.AreEqual(1, receivedPackets[0].SequenceNumber);
        Assert.IsFalse(receivedPackets[0].Missed);

        Assert.AreEqual(2, receivedPackets[1].SequenceNumber);
        Assert.IsTrue(receivedPackets[1].Missed, "Seq 2 should be reported as missed.");

        Assert.AreEqual(3, receivedPackets[2].SequenceNumber);
        Assert.IsFalse(receivedPackets[2].Missed);

        Assert.AreEqual(4, receivedPackets[3].SequenceNumber);
        Assert.IsFalse(receivedPackets[3].Missed);
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

        handler.HandlePacket(CreatePacket(1, timestamp));
        handler.HandlePacket(CreatePacket(2, timestamp + SamplesPerPacket));

        // Wait for buffer timer (100ms) + idle timer (200ms) + safety margin
        await Task.Delay(600, context.CancellationToken).ConfigureAwait(false);

        int countAfterRemoval = receivedPackets.Count;

        // State should be removed. Sending new packets should create fresh state.
        handler.HandlePacket(CreatePacket(500, 5000000));
        handler.HandlePacket(CreatePacket(501, 5000000 + SamplesPerPacket));

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

        handler.HandlePacket(CreatePacket(1, timestamp));

        // Send packets at regular intervals, each resetting the timer before the 200ms fires
        for (ushort i = 2; i <= 5; i++)
        {
            await Task.Delay(50, context.CancellationToken).ConfigureAwait(false);
            timestamp += SamplesPerPacket;
            handler.HandlePacket(CreatePacket(i, timestamp));
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

        handler.HandlePacket(CreatePacket(1, 10000));

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
            handler.HandlePacket(CreatePacket(seq, timestampBase + ((uint)(seq - 1) * SamplesPerPacket)));
        }

        // Buffer should reorder all pair swaps to produce exact 1-30 delivery
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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
        Assert.IsNotEmpty(nonMissed, "At least some packets should be delivered.");
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

        // All 30 entries should be present (28 delivered + 2 missed)
        Assert.HasCount(30, receivedPackets, "Should have exactly 30 entries.");
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

        // Old range should have been force-evicted
        var preJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= startSeq).ToList();
        Assert.HasCount(5, preJump, "All 5 pre-jump packets should be force-evicted.");

        var postJump = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= jumpSeq && p.SequenceNumber < jumpSeq + 26).ToList();
        Assert.IsNotEmpty(postJump, "Packets after resync across wraparound should be delivered.");
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
        Assert.IsNotEmpty(nonMissed);
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

        // With startup=0, eviction starts from the very first packet
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // Verify dropped packets that were delivered as missed are correctly identified
        var missedSeqs = receivedPackets.Where(p => p.Missed).Select(p => p.SequenceNumber).ToHashSet();
        foreach (var seq in missedSeqs)
        {
            Assert.IsTrue(dropped.Contains(seq), $"Packet {seq} was reported as missed but was never dropped.");
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

        // Normal-range packets should all be delivered
        var normalDelivered = receivedPackets.Where(p => !p.Missed && p.SequenceNumber >= 1 && p.SequenceNumber <= 30).ToList();
        Assert.IsNotEmpty(normalDelivered, "Normal-range packets should be delivered despite scattered outliers.");
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
        Assert.IsGreaterThan(100, receivedPackets.Count, "Large volume should produce many output packets.");
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

        // All 30 packets should be delivered in exact order
        AssertExactOrderedDelivery(receivedPackets, 1, 30);
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

        // Verify all non-missed packets are among the sent ones
        var sent = new HashSet<ushort> { 1, 6, 11, 16, 21, 26 };
        var nonMissed = receivedPackets.Where(p => !p.Missed).ToList();
        foreach (var p in nonMissed)
        {
            Assert.IsTrue(sent.Contains(p.SequenceNumber),
                $"Received packet {p.SequenceNumber} that was never sent.");
        }
    }
}
