using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace NetCord.Gateway.Voice;

internal sealed class SpeedNormalizingStream : RewritingStream
{
    private readonly long _timestampFrequency;
    private readonly long _frameDurationTicks;
    private readonly TimeProvider _timeProvider;
    private readonly SyncTimerWaiter _syncWaiter;

    private DelayTaskSource? _delaySource;
    private long _nextTick;
    private bool _used;

    public SpeedNormalizingStream(Stream next, float frameDuration, TimeProvider timeProvider) : base(next)
    {
        var timestampFrequency = _timestampFrequency = timeProvider.TimestampFrequency;
        _frameDurationTicks = (int)(frameDuration * 2) * timestampFrequency / TimeSpan.MillisecondsPerSecond / 2;
        _timeProvider = timeProvider;
        _syncWaiter = new(timeProvider);

        _delaySource = new(timeProvider);
    }

    private void Initialize()
    {
        _used = true;
        _nextTick = _timeProvider.GetTimestamp();
    }

    private long GetDelayTicks()
    {
        var totalTicks = _nextTick - _timeProvider.GetTimestamp();
        var (seconds, ticks) = Math.DivRem(totalTicks, _timestampFrequency);
        return (seconds * TimeSpan.TicksPerSecond) + (ticks * TimeSpan.TicksPerSecond / _timestampFrequency);
    }

    private DelayTaskSource RecreateDelaySource(long delay, DelayTaskSource oldDelaySource, CancellationToken cancellationToken)
    {
        DelayTaskSource newDelaySource = new(_timeProvider);
        var actualDelaySource = Interlocked.CompareExchange(ref _delaySource, newDelaySource, oldDelaySource);

        if (actualDelaySource != oldDelaySource)
        {
            newDelaySource.Dispose();

            if (actualDelaySource is null)
                ThrowObjectDisposed();
            else
                ThrowWriteInProgress();
        }

        oldDelaySource.Dispose();

        if (!newDelaySource.TryReset(new(delay), cancellationToken))
            ThrowWriteInProgress();

        return newDelaySource;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_used)
        {
            var delayTicks = GetDelayTicks();
            if (delayTicks > 0)
            {
                // No Interlocked needed here
                var delaySource = _delaySource;

                if (delaySource is null)
                    ThrowObjectDisposed();

                if (!delaySource.TryReset(new(delayTicks), cancellationToken))
                    delaySource = RecreateDelaySource(delayTicks, delaySource, cancellationToken);

                await new ValueTask(delaySource, delaySource.Version).ConfigureAwait(false);
            }
        }
        else
            Initialize();

        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _nextTick += _frameDurationTicks;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_used)
        {
            var delayTicks = GetDelayTicks();
            if (delayTicks > 0)
                _syncWaiter.Wait(new(delayTicks));
        }
        else
            Initialize();

        _next.Write(buffer);
        _nextTick += _frameDurationTicks;
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        _used = false;
        return base.FlushAsync(cancellationToken);
    }

    public override void Flush()
    {
        _used = false;
        base.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _syncWaiter.Dispose();
            Interlocked.Exchange(ref _delaySource, null)?.Dispose();
        }

        base.Dispose(disposing);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowTimerChangeFailed()
    {
        throw new InvalidOperationException("Failed to change timer.");
    }

    [StackTraceHidden]
    private static ObjectDisposedException GetObjectDisposedException()
    {
        return new ObjectDisposedException(typeof(SpeedNormalizingStream).FullName);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowObjectDisposed()
    {
        throw GetObjectDisposedException();
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowWriteInProgress()
    {
        throw new InvalidOperationException("A write operation is already in progress.");
    }

    private sealed class DelayTaskSource : IValueTaskSource, IDisposable
    {
        private readonly ITimer _timer;
        private ManualResetValueTaskSourceCore<bool> _core;
        private CancellationTokenRegistration _cancellationTokenRegistration;

        // 0 - completed, 1 - waiting, 2 - cancelled, 3 - disposed
        private byte _state;

        public DelayTaskSource(TimeProvider timeProvider)
        {
            TimerCallback callback = timeProvider == TimeProvider.System
                ? static s => Unsafe.As<DelayTaskSource>(s!).TryComplete()
                : static s => ((DelayTaskSource)s!).TryComplete();

            using (ExecutionContext.SuppressFlow())
                _timer = timeProvider.CreateTimer(callback, this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public short Version => _core.Version;

        private void TryComplete()
        {
            if (Interlocked.CompareExchange(ref _state, 0, 1) is 1)
                _core.SetResult(true);
        }

        private void TrySetCanceled(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _state, 2, 1) is 1)
            {
                _timer.Dispose();

                _core.SetException(new OperationCanceledException(cancellationToken));
            }
        }

        public bool TryReset(TimeSpan delay, CancellationToken cancellationToken)
        {
            _core.Reset();

            switch (Interlocked.CompareExchange(ref _state, 1, 0))
            {
                case 0:
                    break;
                case 2:
                    return false;
                case 3:
                    ThrowObjectDisposed();
                    break;
                default:
                    ThrowWriteInProgress();
                    break;
            }

            if (!_timer.Change(delay, Timeout.InfiniteTimeSpan))
                ThrowTimerChangeFailed();

            _cancellationTokenRegistration = cancellationToken.UnsafeRegister(static (s, t) => Unsafe.As<DelayTaskSource>(s!).TrySetCanceled(t), this);

            if (Interlocked.CompareExchange(ref _state, 0, 0) is not 1)
                _cancellationTokenRegistration.Dispose();

            return true;
        }

        public void GetResult(short token)
        {
            _cancellationTokenRegistration.Dispose();

            _core.GetResult(token);
        }

        public ValueTaskSourceStatus GetStatus(short token) => _core.GetStatus(token);

        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags) => _core.OnCompleted(continuation, state, token, flags);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _state, 3) is 1)
                _core.SetException(GetObjectDisposedException());

            _timer.Dispose();
            _cancellationTokenRegistration.Dispose();
        }
    }

    private sealed class SyncTimerWaiter : IDisposable
    {
        private readonly ITimer _timer;
        private readonly ManualResetEventSlim _event;

        // 0 - completed, 1 - waiting, 2 - disposed
        private byte _state;

        public SyncTimerWaiter(TimeProvider timeProvider)
        {
            TimerCallback callback = timeProvider == TimeProvider.System
                ? static s => Unsafe.As<SyncTimerWaiter>(s!).Complete()
                : static s => ((SyncTimerWaiter)s!).Complete();

            using (ExecutionContext.SuppressFlow())
                _timer = timeProvider.CreateTimer(callback, this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _event = new(false, 0);
        }

        private void Complete()
        {
            // Can be used after disposal, so no need to check the '_state',
            // see https://github.com/dotnet/runtime/issues/89692
            _event.Set();
        }

        public void Wait(TimeSpan delay)
        {
            _event.Reset();

            switch (Interlocked.CompareExchange(ref _state, 1, 0))
            {
                case 0:
                    break;
                case 2:
                    ThrowObjectDisposed();
                    break;
                default:
                    ThrowWriteInProgress();
                    break;
            }

            if (!_timer.Change(delay, Timeout.InfiniteTimeSpan))
                ThrowTimerChangeFailed();

            _event.Wait();

            if (Interlocked.CompareExchange(ref _state, 0, 1) is 2)
                ThrowObjectDisposed();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _state, 2) is 1)
                _event.Set();

            _timer.Dispose();
            _event.Dispose();
        }
    }
}
