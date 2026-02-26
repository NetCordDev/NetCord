using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace NetCord.Gateway.Voice;

public sealed class BufferedVoiceReceiveHandler : VoiceReceiveHandler
{
    private const int MinSamplesPerPacket = 5 * (48000 / 1000) / 2;

    private readonly int _bufferDuration;
    private readonly int _bufferSize;
    private readonly int _bufferSamples;
    private readonly int _startupSize;
    private readonly int _startupSamples;
    private readonly int _resynchronizationSamples;
    private readonly short _minResynchronizationPackets;
    private readonly int _resynchronizationThreshold;
    private readonly int _idleTimeout;

    private readonly Dictionary<uint, JitterState> _state = [];
    private readonly ReaderWriterLockSlim _stateLock = new();

    public BufferedVoiceReceiveHandler(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        var bufferDuration = configuration?.BufferDuration ?? 240;
        var startupDuration = configuration?.StartupDuration ?? (bufferDuration / 2);
        var resynchronizationDuration = configuration?.ResynchronizationDuration ?? (2 * bufferDuration);
        var resynchronizationThreshold = configuration?.ResynchronizationThreshold ?? 4;
        var idleTimeout = configuration?.IdleTimeout ?? 60_000;

        var bufferSize = (int)((long)bufferDuration * 2 / 5);

        if (bufferSize <= 0)
            ThrowInvalidBufferDuration();

        var bufferSamples = bufferSize * MinSamplesPerPacket;

        if (bufferSamples <= 0)
            ThrowInvalidBufferDuration();

        var startupSize = startupDuration * 2 / 5;

        if (startupSize > bufferSize)
            ThrowInvalidStartupDuration();

        var startupSamples = startupSize * MinSamplesPerPacket;

        if (startupSamples > bufferSamples)
            ThrowInvalidStartupDuration();

        var minResynchronizationPacketsInt = (int)((long)resynchronizationDuration * 2 / 5);

        if ((uint)minResynchronizationPacketsInt > short.MaxValue)
            ThrowInvalidResynchronizationDuration();

        var minResynchronizationPackets = (short)minResynchronizationPacketsInt;

        var resynchronizationSamples = minResynchronizationPackets * MinSamplesPerPacket;

        if (resynchronizationSamples < bufferSamples)
            ThrowInvalidResynchronizationDuration();

        if (resynchronizationThreshold <= 0)
            ThrowInvalidResynchronizationThreshold();

        if (idleTimeout <= 0)
            ThrowInvalidIdleTimeout();

        _bufferDuration = bufferDuration;
        _bufferSize = bufferSize;
        _bufferSamples = bufferSamples;
        _startupSize = startupSize;
        _startupSamples = startupSamples;
        _resynchronizationSamples = resynchronizationSamples;
        _minResynchronizationPackets = minResynchronizationPackets;
        _resynchronizationThreshold = resynchronizationThreshold;
        _idleTimeout = idleTimeout;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidBufferDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}' cannot be lower than or equal to 0 and cannot be greater than 44739244.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidStartupDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"{nameof(BufferedVoiceReceiveHandlerConfiguration.StartupDuration)}' cannot be lower than or equal to 0 and must be less than or equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}'. By default '{nameof(BufferedVoiceReceiveHandlerConfiguration.StartupDuration)}' is equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)} / 2'.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidResynchronizationDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.ResynchronizationDuration)}' cannot be lower than or equal to 0 and must be greater than or equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}' as well as cannot be greater than 44739244. By default '{nameof(BufferedVoiceReceiveHandlerConfiguration.ResynchronizationDuration)}' is equal to '2 * {nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}'.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidResynchronizationThreshold()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.ResynchronizationThreshold)}' must be greater than 0.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidIdleTimeout()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.IdleTimeout)}' must be greater than 0.");
    }

    public override bool RequiresExternalSocketAddress => true;

    public override void Handle(VoiceReceiveContext context)
    {
        var packet = context.Packet;
        var ssrc = packet.Ssrc;

        bool found;
        JitterState? state;

        _stateLock.EnterReadLock();
        try
        {
            found = _state.TryGetValue(ssrc, out state);
        }
        finally
        {
            _stateLock.ExitReadLock();
        }

        if (!found || !state!.Update(this, context))
        {
            state = new(this, ssrc, _bufferSize);

            state.Initialize(this, context);

            _stateLock.EnterWriteLock();
            try
            {
                _state[ssrc] = state;
            }
            finally
            {
                _stateLock.ExitWriteLock();
            }
        }
    }

    private void RemoveState(uint ssrc)
    {
        _stateLock.EnterWriteLock();
        try
        {
            _state.Remove(ssrc);
        }
        finally
        {
            _stateLock.ExitWriteLock();
        }
    }

    private sealed class JitterState
    {
        [DebuggerDisplay("{Buffer,nq}")]
        private readonly struct StoredContext : IDisposable
        {
            public StoredContext(VoiceReceiveContext context)
            {
                var packet = context.Packet;
                var datagram = packet.Datagram;

                var frame = context.Frame;

                var buffer = ArrayPool<byte>.Shared.Rent(datagram.Length + frame.Length);
                datagram.CopyTo(buffer);
                frame.CopyTo(buffer.AsSpan(datagram.Length));

                Buffer = buffer;
                PacketLength = datagram.Length;
            }

            public byte[]? Buffer { get; }

            public int PacketLength { get; }

            public int FrameLength { get; }

            public bool TryGetData(out RtpPacket packet, out ReadOnlySpan<byte> frame)
            {
                if (Buffer is not { } buffer)
                {
                    packet = default;
                    frame = default;
                    return false;
                }

                packet = new(buffer.AsSpan(0, PacketLength));
                frame = buffer.AsSpan(PacketLength, FrameLength);
                return true;
            }

            public void Dispose()
            {
                if (Buffer is not null)
                    ArrayPool<byte>.Shared.Return(Buffer);
            }
        }

        private readonly StoredContext[] _contexts;
        private readonly Timer _stopTimer;
        private readonly Lock _lock = new();

        private ushort _latestPacketSequenceNumber;
        private uint _latestPacketTimestamp;
        private int _latestPacketIndex;

        private ushort _lastEvictedSequenceNumber;
        private uint _lastEvictedPacketTimestamp;
        private bool _anyEvicted;
        private bool _isLastLost;

        private ushort _lastOutlierSequenceNumber;
        private uint _lastOutlierTimestamp;
        private int _outlierCount;

        private ActiveState _state;

        private enum ActiveState : byte
        {
            Active,
            Idle,
            Disposed,
        }

        public JitterState(BufferedVoiceReceiveHandler owner, uint ssrc, int bufferSize)
        {
            _contexts = ArrayPool<StoredContext>.Shared.Rent(bufferSize);
            _stopTimer = new(OnStop, Tuple.Create(owner, this, ssrc), Timeout.Infinite, Timeout.Infinite);
        }

        private void Dispose()
        {
            ArrayPool<StoredContext>.Shared.Return(_contexts);

            _stopTimer.Dispose();
        }

        private static void OnStop(object? state)
        {
            var (owner, jitterState, ssrc) = Unsafe.As<Tuple<BufferedVoiceReceiveHandler, JitterState, uint>>(state!);

            // If we can't acquire the lock, it means the state is currently being updated, so we can skip eviction
            if (jitterState._lock.TryEnter())
            {
                try
                {
                    switch (jitterState._state)
                    {
                        case ActiveState.Active:
                            jitterState.ForceEvictAll(owner, ssrc);

                            jitterState._state = ActiveState.Idle;

                            jitterState._stopTimer.Change(owner._idleTimeout, Timeout.Infinite);
                            break;
                        case ActiveState.Idle:
                            owner.RemoveState(ssrc);

                            jitterState._state = ActiveState.Disposed;

                            jitterState.Dispose();

                            Console.WriteLine("Removed");
                            break;
                    }
                }
                finally
                {
                    jitterState._lock.Exit();
                }
            }
        }

        private int GetPacketIndex(BufferedVoiceReceiveHandler owner, ushort sequenceNumber)
        {
            var diff = (short)(sequenceNumber - _latestPacketSequenceNumber);

            var index = (_latestPacketIndex + diff) % owner._bufferSize;

            if (index < 0)
                index += owner._bufferSize;

            return index;
        }

        private int GetMaxStoredPacketCount(ushort sequenceNumber)
        {
            return (short)(sequenceNumber - _lastEvictedSequenceNumber);
        }

        private bool IsNotInWindowRange(BufferedVoiceReceiveHandler owner, short sequenceNumberDiff, int timestampDiff)
        {
            return sequenceNumberDiff <= (short)(_lastEvictedSequenceNumber - _latestPacketSequenceNumber)
                || sequenceNumberDiff > owner._minResynchronizationPackets
                || timestampDiff <= (int)(_lastEvictedPacketTimestamp - _latestPacketTimestamp)
                || timestampDiff > owner._resynchronizationSamples;
        }

        private static bool IsNotInBufferRange(BufferedVoiceReceiveHandler owner, short sequenceNumberDiff, int timestampDiff)
        {
            return sequenceNumberDiff <= -owner._bufferSize
                || sequenceNumberDiff > owner._minResynchronizationPackets
                || timestampDiff <= -owner._bufferSamples
                || timestampDiff > owner._resynchronizationSamples;
        }

        public void Initialize(BufferedVoiceReceiveHandler owner, VoiceReceiveContext context)
        {
            var packet = context.Packet;
            var sequenceNumber = packet.SequenceNumber;
            var timestamp = packet.Timestamp;

            _latestPacketSequenceNumber = sequenceNumber;
            _latestPacketTimestamp = timestamp;
            _latestPacketIndex = 0;

            _anyEvicted = false;
            _isLastLost = false;
            _lastEvictedSequenceNumber = (ushort)(sequenceNumber - (uint)owner._bufferSize + (uint)owner._startupSize - 1);
            _lastEvictedPacketTimestamp = timestamp - (uint)owner._bufferSamples + (uint)owner._startupSamples - MinSamplesPerPacket;

            _outlierCount = 0;

            _contexts[0] = new(context);

            _stopTimer.Change(owner._bufferDuration, Timeout.Infinite);
        }

        private void ForceEvict(BufferedVoiceReceiveHandler owner, uint ssrc, uint timestamp, ushort sequenceNumber)
        {
            var count = GetMaxStoredPacketCount(sequenceNumber) - owner._bufferSize;

            var baseSequenceNumber = _lastEvictedSequenceNumber;

            for (int i = 1; i <= count; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                var index = GetPacketIndex(owner, expectedSeq);
                var storedPacket = _contexts[index];

                if (storedPacket.TryGetData(out var packet, out var frame) && packet.SequenceNumber == expectedSeq)
                    EvictStoredPacket(owner, index, storedPacket, packet, frame);
                else
                {
                    if (_anyEvicted)
                    {
                        owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Lost(ssrc, (ushort)(expectedSeq - 1)));
                        _lastEvictedSequenceNumber = (ushort)(expectedSeq - 1);
                    }

                    _lastEvictedSequenceNumber = (ushort)(expectedSeq - 1);
                }
                
                // EvictSequenceNumber(owner, ssrc, expectedSeq);
            }

            count = GetMaxStoredPacketCount(sequenceNumber);

            baseSequenceNumber = _lastEvictedSequenceNumber;

            for (int i = 1; i < count; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                var index = GetPacketIndex(owner, expectedSeq);
                var storedPacket = _contexts[index];

                if (storedPacket.TryGetData(out var packet, out var frame) && packet.SequenceNumber == expectedSeq)
                {
                    if ((int)(timestamp - _lastEvictedPacketTimestamp) <= owner._bufferSamples)
                        break;

                    if (_anyEvicted)
                    {
                        var sequenceNumberDiff = (int)(short)(expectedSeq - _lastEvictedSequenceNumber);
                        var timestampDiff = (int)(packet.Timestamp - _lastEvictedPacketTimestamp);
                        var samplesPerPacket = timestampDiff / sequenceNumberDiff;
                        for (int k = 1; k < sequenceNumberDiff; k++)
                            owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Lost(ssrc,
                                                                                (uint)(packet.Timestamp - (samplesPerPacket * k)),
                                                                                (ushort)(_lastEvictedSequenceNumber + k),
                                                                                samplesPerPacket));
                    }

                    EvictStoredPacket(owner, index, storedPacket, packet, frame);
                }
            }
        }

        private void ForceEvictAll(BufferedVoiceReceiveHandler owner, uint ssrc)
        {
            var count = GetMaxStoredPacketCount(_latestPacketSequenceNumber);

            var baseSequenceNumber = _lastEvictedSequenceNumber;

            for (int i = 1; i <= count; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                EvictSequenceNumber(owner, ssrc, expectedSeq);
            }
        }

        private void EvictSequenceNumber(BufferedVoiceReceiveHandler owner, uint ssrc, ushort expectedSeq)
        {
            var index = GetPacketIndex(owner, expectedSeq);
            var storedPacket = _contexts[index];

            if (storedPacket.TryGetData(out var packet, out var frame) && packet.SequenceNumber == expectedSeq)
                EvictStoredPacket(owner, index, storedPacket, packet, frame);
            else
            {
                if (_anyEvicted)
                {
                    owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Lost(ssrc, (ushort)(expectedSeq - 1)));
                    _lastEvictedSequenceNumber = (ushort)(expectedSeq - 1);
                    // if (_isLastLost)
                    // {
                    //     owner.InvokeVoiceReceive(VoiceReceiveData.Lost(ssrc, (ushort)(expectedSeq - 1)));
                    //     _lastEvictedSequenceNumber = (ushort)(expectedSeq - 1);
                    // }
                    // else
                    //     _isLastLost = true;
                }
                // else
                _lastEvictedSequenceNumber = (ushort)(expectedSeq - 1);
            }

            // var index = GetPacketIndex(owner, expectedSeq);
            // var storedPacket = _packets[index];

            // if (storedPacket.TryGetPacket(out var packet) && packet.SequenceNumber == expectedSeq)
            //     EvictStoredPacket(owner, index, storedPacket, packet);
            // else
            // {
            //     if (_anyEvicted)
            //         owner.InvokeVoiceReceive(VoiceReceiveData.Lost(ssrc, expectedSeq));

            //     _lastEvictedSequenceNumber = expectedSeq;
            // }
        }

        private void EvictStoredPacket(BufferedVoiceReceiveHandler owner, int index, StoredContext storedPacket, RtpPacket packet, ReadOnlySpan<byte> frame)
        {
            owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Delivered(frame, packet.Ssrc, packet.Timestamp, packet.SequenceNumber));

            _isLastLost = false;

            _lastEvictedSequenceNumber = packet.SequenceNumber;
            _lastEvictedPacketTimestamp = packet.Timestamp;
            _anyEvicted = true;

            _contexts[index] = default;
            storedPacket.Dispose();
        }

        private void EvictReady(BufferedVoiceReceiveHandler owner, VoiceReceiveContext context, int packetIndex, bool isForward, out bool evictedCurrentPacket)
        {
            evictedCurrentPacket = false;

            var packet = context.Packet;

            var updatedSequenceNumber = isForward ? packet.SequenceNumber : _latestPacketSequenceNumber;

            while (GetMaxStoredPacketCount(updatedSequenceNumber) > 0)
            {
                var expectedSeq = (ushort)(_lastEvictedSequenceNumber + 1);

                var evictedIndex = GetPacketIndex(owner, expectedSeq);

                RtpPacket evictedPacket;
                ReadOnlySpan<byte> evictedFrame;
                StoredContext evictedStoredPacket;

                var isCurrentIndex = evictedIndex == packetIndex;

                if (isCurrentIndex)
                {
                    evictedPacket = packet;
                    evictedFrame = context.Frame;
                    Unsafe.SkipInit(out evictedStoredPacket);
                }
                else
                {
                    evictedStoredPacket = _contexts[evictedIndex];

                    if (!evictedStoredPacket.TryGetData(out evictedPacket, out evictedFrame))
                        break;
                }

                owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Delivered(evictedFrame,
                                                                         evictedPacket.Ssrc,
                                                                         evictedPacket.Timestamp,
                                                                         evictedPacket.SequenceNumber));

                _isLastLost = false;

                _lastEvictedSequenceNumber = evictedPacket.SequenceNumber;
                _lastEvictedPacketTimestamp = evictedPacket.Timestamp;

                if (isCurrentIndex)
                    evictedCurrentPacket = true;
                else
                {
                    _contexts[evictedIndex] = default;
                    evictedStoredPacket.Dispose();
                }
            }
        }

        public bool Update(BufferedVoiceReceiveHandler owner, VoiceReceiveContext context)
        {
            using var lockScope = _lock.EnterScope();

            if (_state is ActiveState.Disposed)
                return false;

            var packet = context.Packet;
            var packetSequenceNumber = packet.SequenceNumber;

            var sequenceNumberDiff = (short)(packetSequenceNumber - _latestPacketSequenceNumber);

            var packetTimestamp = packet.Timestamp;

            var timestampDiff = (int)(packetTimestamp - _latestPacketTimestamp);

            if (IsNotInWindowRange(owner, sequenceNumberDiff, timestampDiff))
            {
                if (_outlierCount is 0 || IsNotInBufferRange(owner,
                                                             (short)(packetSequenceNumber - _lastOutlierSequenceNumber),
                                                             (int)(packetTimestamp - _lastOutlierTimestamp)))
                    _outlierCount = 1;
                else
                    _outlierCount++;

                _lastOutlierSequenceNumber = packetSequenceNumber;
                _lastOutlierTimestamp = packetTimestamp;

                if (_outlierCount >= owner._resynchronizationThreshold)
                {
                    ForceEvictAll(owner, packet.Ssrc);

                    // Reinitialize
                    Initialize(owner, context);
                }

                goto Ret;
            }

            var packetIndex = GetPacketIndex(owner, packetSequenceNumber);

            // Seems to be a duplicate packet, ignore it
            if (_contexts[packetIndex].TryGetData(out var existingPacket, out var existingFrame) && existingPacket.SequenceNumber == packetSequenceNumber)
                goto Ret;

            _outlierCount = 0;

            ForceEvict(owner, packet.Ssrc, packetTimestamp, packetSequenceNumber);

            bool evictedCurrentPacket;

            var isForward = sequenceNumberDiff > 0;

            // Only evict ready packets if there has been at least one eviction already,
            // this means we will only evict ready packets if the buffer has been filled at least once
            if (_anyEvicted)
                EvictReady(owner, context, packetIndex, isForward, out evictedCurrentPacket);
            else
                evictedCurrentPacket = false;

            if (!evictedCurrentPacket)
                _contexts[packetIndex] = new(context);

            if (isForward)
            {
                _latestPacketSequenceNumber = packetSequenceNumber;
                _latestPacketTimestamp = packetTimestamp;
                _latestPacketIndex = packetIndex;
            }

            Ret:
            _state = ActiveState.Active;

            _stopTimer.Change(owner._bufferDuration, Timeout.Infinite);

            return true;
        }
    }
}

public class BufferedVoiceReceiveHandlerConfiguration
{
    public int? BufferDuration { get; set; }

    public int? StartupDuration { get; set; }

    public int? ResynchronizationDuration { get; set; }

    public int? ResynchronizationThreshold { get; set; }

    public int? IdleTimeout { get; set; }
}
