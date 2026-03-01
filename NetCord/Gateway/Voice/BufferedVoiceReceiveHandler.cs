using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public sealed class BufferedVoiceReceiveHandler : VoiceReceiveHandler
{
    private const int Fs = 48_000;
    private const int MinSamplesPerPacket = 5 * (Fs / 1000) / 2;
    private const int MaxSamplesPerPacket = 120 * (Fs / 1000);

    private readonly int _bufferDuration;
    private readonly int _bufferSize;
    private readonly int _bufferSamples;
    private readonly int _startupSize;
    private readonly int _startupSamples;
    private readonly int _resynchronizationSamples;
    private readonly short _minResynchronizationPackets;
    private readonly int _resynchronizationThreshold;
    private readonly int _idleTimeout;

    private readonly Dictionary<uint, State> _state = [];
    private readonly ReaderWriterLockSlim _stateLock = new();

    public BufferedVoiceReceiveHandler(BufferedVoiceReceiveHandlerConfiguration? configuration = null)
    {
        var bufferDuration = configuration?.BufferDuration ?? 240;
        var startupDuration = configuration?.StartupDuration ?? (bufferDuration / 2);
        var resynchronizationDuration = configuration?.ResynchronizationDuration ?? (2 * bufferDuration);
        var resynchronizationThreshold = configuration?.ResynchronizationThreshold ?? 4;
        var idleTimeout = configuration?.IdleTimeout ?? 60_000;

        if (bufferDuration <= 2)
            ThrowInvalidBufferDuration();

        if (startupDuration < 0 || startupDuration > bufferDuration)
            ThrowInvalidStartupDuration();

        if (resynchronizationDuration <= 2 || resynchronizationDuration < bufferDuration)
            ThrowInvalidResynchronizationDuration();

        if (resynchronizationThreshold <= 0)
            ThrowInvalidResynchronizationThreshold();

        if (idleTimeout <= 0)
            ThrowInvalidIdleTimeout();

        var bufferSizeLong = (long)bufferDuration * 2 / 5;
        var bufferSamplesLong = bufferSizeLong * MinSamplesPerPacket;

        if (bufferSamplesLong > int.MaxValue)
            ThrowInvalidBufferDuration();

        var resyncPacketsLong = (long)resynchronizationDuration * 2 / 5;
        
        if (resyncPacketsLong > short.MaxValue)
            ThrowInvalidResynchronizationDuration();

        _bufferDuration = bufferDuration;
        _bufferSize = (int)bufferSizeLong;
        _bufferSamples = (int)bufferSamplesLong;

        _startupSize = (int)((long)startupDuration * 2 / 5);
        _startupSamples = _startupSize * MinSamplesPerPacket;

        _minResynchronizationPackets = (short)resyncPacketsLong;
        _resynchronizationSamples = _minResynchronizationPackets * MinSamplesPerPacket;
        _resynchronizationThreshold = resynchronizationThreshold;
        _idleTimeout = idleTimeout;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidBufferDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}' cannot be lower than or equal to 2 and cannot be greater than 44739244.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidStartupDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.StartupDuration)}' cannot be lower than 0 and must be less than or equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}'. By default '{nameof(BufferedVoiceReceiveHandlerConfiguration.StartupDuration)}' is equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)} / 2'.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidResynchronizationDuration()
    {
        throw new ArgumentOutOfRangeException("configuration", $"'{nameof(BufferedVoiceReceiveHandlerConfiguration.ResynchronizationDuration)}' cannot be lower than or equal to 2 and must be greater than or equal to '{nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}' and cannot be greater than 81919. By default '{nameof(BufferedVoiceReceiveHandlerConfiguration.ResynchronizationDuration)}' is equal to '2 * {nameof(BufferedVoiceReceiveHandlerConfiguration.BufferDuration)}'.");
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
        State? state;

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

    private sealed class State
    {
        [StructLayout(LayoutKind.Auto)]
        [DebuggerDisplay("{Buffer,nq}")]
        private readonly struct StoredContext : IDisposable
        {
            public StoredContext(VoiceReceiveContext context)
            {
                var frame = context.Frame;
                var frameLength = frame.Length;

                var frameCopy = ArrayPool<byte>.Shared.Rent(frameLength);
                frame.CopyTo(frameCopy);
                
                _frame = frameCopy;
                _frameLength = frameLength;

                var packet = context.Packet;
                _timestamp = packet.Timestamp;
                _sequenceNumber = packet.SequenceNumber;
            }

            private readonly byte[]? _frame;

            private readonly int _frameLength;

            private readonly uint _timestamp;

            private readonly ushort _sequenceNumber;

            public bool TryGetData(out ContextData data)
            {
                if (_frame is not { } frame)
                {
                    data = default;
                    return false;
                }

                data = new(_sequenceNumber, _timestamp, frame.AsSpan(0, _frameLength));
                return true;
            }

            public void Dispose()
            {
                if (_frame is { } frame)
                    ArrayPool<byte>.Shared.Return(frame);
            }
        }

        private readonly ref struct ContextData(ushort sequenceNumber, uint timestamp, ReadOnlySpan<byte> frame)
        {
            public ushort SequenceNumber => sequenceNumber;

            public uint Timestamp => timestamp;

            public ReadOnlySpan<byte> Frame { get; } = frame;
        }

        private readonly StoredContext[] _contexts;
        private readonly Timer _stopTimer;
        private readonly Lock _lock = new();

        private ushort _latestPacketSequenceNumber;
        private uint _latestPacketTimestamp;
        private int _latestPacketIndex;

        private ushort _lastEvictedSequenceNumber;
        private uint _lastEvictedPacketTimestamp;
        private int _lastEvictedPacketSamples;
        private bool _anyEvicted;

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

        public State(BufferedVoiceReceiveHandler owner, uint ssrc, int bufferSize)
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
            var (owner, jitterState, ssrc) = Unsafe.As<Tuple<BufferedVoiceReceiveHandler, State, uint>>(state!);

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

        private static bool IsNotInSync(int timestampDiff)
        {
            return timestampDiff % MinSamplesPerPacket is not 0;
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
            _lastEvictedSequenceNumber = (ushort)(sequenceNumber - (uint)owner._bufferSize + (uint)owner._startupSize - 1);
            _lastEvictedPacketTimestamp = timestamp - (uint)owner._bufferSamples + (uint)owner._startupSamples - MinSamplesPerPacket;

            _outlierCount = 0;

            _contexts[0] = new(context);

            _stopTimer.Change(owner._bufferDuration, Timeout.Infinite);
        }

#region Based on https://github.com/xiph/opus/blob/8161640db03727aa9a0d76377d16e5288b7b2342/src/opus.c
        private static int GetNumberOfFrames(ReadOnlySpan<byte> frame)
        {
            int count = frame[0]&0x3;
            if (count==0)
                return 1;
            else if (count!=3)
                return 2;
            else
                return frame[1] & 0x3F;
        }

        private static int GetSamplesPerFrame(ReadOnlySpan<byte> frame)
        {
            var data = frame;
            int audiosize;
            if ((data [0] & 0x80) is not 0)
            {
                audiosize = (data[0] >> 3) & 0x3;
                audiosize = (Fs << audiosize) / 400;
            } else if ((data[0] & 0x60) == 0x60)
            {
                audiosize = ((data[0] & 0x08) is not 0) ? Fs / 50 : Fs / 100;
            }
            else {
                audiosize = (data[0] >> 3) & 0x3;
                if (audiosize == 3)
                    audiosize = Fs * 60 / 1000;
                else
                    audiosize = (Fs << audiosize) / 100;
            }
            return audiosize;
        }

        private static int GetSamplesPerChannel(ReadOnlySpan<byte> frame)
        {
            var count = GetNumberOfFrames(frame);
            var samplesPerFrame = GetSamplesPerFrame(frame);
            return count * samplesPerFrame;
        }
#endregion

        private void Push(BufferedVoiceReceiveHandler owner, VoiceReceiveContext context, bool isForward)
        {
            var ssrc = context.Packet.Ssrc;

            var sequenceNumber = context.Packet.SequenceNumber;

            var timestamp = context.Packet.Timestamp;

            var relativeCount = GetMaxStoredPacketCount(sequenceNumber);

            var allCount = isForward ? relativeCount : GetMaxStoredPacketCount(_latestPacketSequenceNumber);

            var countExceedingBuffer = relativeCount - owner._bufferSize;

            var baseSequenceNumber = _lastEvictedSequenceNumber;

            int i = 1;
            for (; i <= countExceedingBuffer; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                var index = GetPacketIndex(owner, expectedSeq);
                var storedPacket = _contexts[index];

                if (storedPacket.TryGetData(out var data) && data.SequenceNumber == expectedSeq)
                {
                    if (_anyEvicted)
                        EvictLostFrames(owner, ssrc, data.Timestamp, data.Frame);

                    EvictStoredPacket(owner, ssrc, index, storedPacket, data);
                }
            }

            var isReady = true;
            
            for (; i < relativeCount; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                var index = GetPacketIndex(owner, expectedSeq);
                var storedPacket = _contexts[index];

                if (storedPacket.TryGetData(out var data) && data.SequenceNumber == expectedSeq)
                {
                    if ((int)(timestamp - _lastEvictedPacketTimestamp) <= owner._bufferSamples)
                        break;

                    isReady = true;

                    if (_anyEvicted)
                        EvictLostFrames(owner, ssrc, data.Timestamp, data.Frame);

                    EvictStoredPacket(owner, ssrc, index, storedPacket, data);
                }
                else
                    isReady = false;
            }

            var currentIndex = GetPacketIndex(owner, sequenceNumber);

            if (!isReady || !_anyEvicted)
            {
                _contexts[currentIndex] = new(context);
                return;
            }

            var evictedCurrentPacket = false;

            for (; i <= allCount; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                
                var evictedIndex = GetPacketIndex(owner, expectedSeq);

                var isCurrentIndex = evictedIndex == currentIndex;

                uint evictedTimestamp;
                ushort evictedSequenceNumber;
                ReadOnlySpan<byte> evictedFrame;
                StoredContext evictedStoredPacket;

                if (isCurrentIndex)
                {
                    evictedTimestamp = context.Packet.Timestamp;
                    evictedSequenceNumber = context.Packet.SequenceNumber;
                    evictedFrame = context.Frame;
                    Unsafe.SkipInit(out evictedStoredPacket);
                }
                else
                {
                    evictedStoredPacket = _contexts[evictedIndex];

                    if (!evictedStoredPacket.TryGetData(out var data))
                        break;

                    evictedTimestamp = data.Timestamp;
                    evictedSequenceNumber = data.SequenceNumber;
                    evictedFrame = data.Frame;
                }

                EvictLostFrames(owner, ssrc, evictedTimestamp, evictedFrame);

                owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Delivered(evictedFrame,
                                                                         ssrc,
                                                                         evictedTimestamp,
                                                                         evictedSequenceNumber));

                _lastEvictedSequenceNumber = evictedSequenceNumber;
                _lastEvictedPacketTimestamp = evictedTimestamp;
                _lastEvictedPacketSamples = GetSamplesPerChannel(evictedFrame);

                if (isCurrentIndex)
                    evictedCurrentPacket = true;
                else
                {
                    _contexts[evictedIndex] = default;
                    evictedStoredPacket.Dispose();
                }
            }

            if (!evictedCurrentPacket)
                _contexts[currentIndex] = new(context);
        }

        private void ForceEvictAll(BufferedVoiceReceiveHandler owner, uint ssrc)
        {
            var count = GetMaxStoredPacketCount(_latestPacketSequenceNumber);

            var baseSequenceNumber = _lastEvictedSequenceNumber;

            for (int i = 1; i <= count; i++)
            {
                var expectedSeq = (ushort)(baseSequenceNumber + i);
                var index = GetPacketIndex(owner, expectedSeq);
                var storedPacket = _contexts[index];

                if (storedPacket.TryGetData(out var data) && data.SequenceNumber == expectedSeq)
                {
                    if (_anyEvicted)
                        EvictLostFrames(owner, ssrc, data.Timestamp, data.Frame);

                    EvictStoredPacket(owner, ssrc, index, storedPacket, data);
                }
            }
        }

        private void EvictLostFrames(BufferedVoiceReceiveHandler owner, uint ssrc, uint timestamp, ReadOnlySpan<byte> fecData)
        {
            // Does not depend on sequence numbers, only on timestamps

            // Packets are evicted in the way the first one is
            // the remainder and the following packets are of max size
            // (120 ms), this allows for FEC decoding to never fail
            // due to the buffer being too small

            var startTimestamp = _lastEvictedPacketTimestamp + (uint)_lastEvictedPacketSamples;
            var timestampDiff = (int)(timestamp - startTimestamp);

            if (timestampDiff <= 0)
                return;

            var firstPacketSamples = timestampDiff % MaxSamplesPerPacket;

            int i = 1;

            if (firstPacketSamples is not 0)
            {
                var currentTimestamp = startTimestamp;
                startTimestamp += (uint)firstPacketSamples;

                var isLast = startTimestamp >= timestamp;

                var currentFecData = isLast ? fecData : default;

                owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Lost(ssrc,
                                                                    currentTimestamp,
                                                                    (ushort)(_lastEvictedSequenceNumber + 1),
                                                                    firstPacketSamples,
                                                                    currentFecData));

                if (isLast)
                    return;

                i++;
            }

            while (true)
            {
                var currentTimestamp = startTimestamp;
                startTimestamp += MaxSamplesPerPacket;
                
                var isLast = startTimestamp >= timestamp;

                var currentFecData = isLast ? fecData : default;

                owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Lost(ssrc,
                                                                    currentTimestamp,
                                                                    (ushort)(_lastEvictedSequenceNumber + i),
                                                                    MaxSamplesPerPacket,
                                                                    currentFecData));

                if (isLast)
                    break;

                i++;
            }
        }

        private void EvictStoredPacket(BufferedVoiceReceiveHandler owner, uint ssrc, int index, StoredContext storedPacket, ContextData data)
        {
            owner.InvokeVoiceReceive(VoiceReceiveEventArgs.Delivered(data.Frame, ssrc, data.Timestamp, data.SequenceNumber));

            _lastEvictedSequenceNumber = data.SequenceNumber;
            _lastEvictedPacketTimestamp = data.Timestamp;
            _lastEvictedPacketSamples = GetSamplesPerChannel(data.Frame);
            _anyEvicted = true;

            _contexts[index] = default;
            storedPacket.Dispose();
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

            if (IsNotInWindowRange(owner, sequenceNumberDiff, timestampDiff) || IsNotInSync(timestampDiff))
            {
                HandleOutlier(owner, context);

                goto Ret;
            }

            var packetIndex = GetPacketIndex(owner, packetSequenceNumber);

            // Seems to be a duplicate packet, ignore it
            if (_contexts[packetIndex].TryGetData(out var data) && data.SequenceNumber == packetSequenceNumber)
                goto Ret;

            _outlierCount = 0;

            var isForward = sequenceNumberDiff > 0;

            Push(owner, context, isForward);

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

        private void HandleOutlier(BufferedVoiceReceiveHandler owner, VoiceReceiveContext context)
        {
            var packet = context.Packet;
            var packetSequenceNumber = packet.SequenceNumber;
            var packetTimestamp = packet.Timestamp;

            if (_outlierCount is 0
                || IsNotInBufferRange(owner,
                                      (short)(packetSequenceNumber - _lastOutlierSequenceNumber),
                                      (int)(packetTimestamp - _lastOutlierTimestamp))
                || IsNotInSync((int)(packetTimestamp - _lastOutlierTimestamp)))
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
