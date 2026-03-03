using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace NetCord.Gateway.Voice;

internal sealed class SpeedNormalizingStream : RewritingStream
{
    private readonly long _timestampFrequency;
    private readonly long _frameDurationTicks;
    private DelayTaskSource _delaySource;
    private readonly TimeProvider _timeProvider;

    private long _nextTick;
    private bool _used;

    public SpeedNormalizingStream(Stream next, float frameDuration, TimeProvider timeProvider) : base(next)
    {
        var timestampFrequency = _timestampFrequency = timeProvider.TimestampFrequency;
        _frameDurationTicks = (int)(frameDuration * 2) * timestampFrequency / TimeSpan.MillisecondsPerSecond / 2;
        _delaySource = new(timeProvider);
        _timeProvider = timeProvider;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_used)
        {
            var delayTicks = _nextTick - _timeProvider.GetTimestamp();
            var delay = delayTicks * TimeSpan.TicksPerSecond / _timestampFrequency;
            if (delay >= TimeSpan.TicksPerMillisecond)
            {
                var delaySource = _delaySource;

                if (!delaySource.TryReset(new(delay), cancellationToken))
                {
                    delaySource.Dispose();

                    delaySource = _delaySource = new(_timeProvider);

                    // Will never return false
                    delaySource.TryReset(new(delay), cancellationToken);
                }

                await new ValueTask(delaySource, delaySource.Version).ConfigureAwait(false);
            }
        }
        else
        {
            _used = true;
            _nextTick = _timeProvider.GetTimestamp();
        }

        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _nextTick += _frameDurationTicks;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_used)
        {
            var delayTicks = _nextTick - _timeProvider.GetTimestamp();
            var delay = (int)(delayTicks * TimeSpan.MillisecondsPerSecond / _timestampFrequency);
            if (delay > 0)
                Thread.Sleep(delay);
        }
        else
        {
            _used = true;
            _nextTick = _timeProvider.GetTimestamp();
        }

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
            _delaySource.Dispose();

        base.Dispose(disposing);
    }

    private sealed class DelayTaskSource : IValueTaskSource, IDisposable
    {
        private readonly ITimer _timer;
        private ManualResetValueTaskSourceCore<bool> _core;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private byte _state;

        public DelayTaskSource(TimeProvider timeProvider)
        {
            using (ExecutionContext.SuppressFlow())
                _timer = timeProvider.CreateTimer(static s => ((DelayTaskSource)s!).TryComplete(), this, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public short Version => _core.Version;

        private void TryComplete()
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) is 0)
                _core.SetResult(true);
        }

        private void TrySetCanceled(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _state, 2, 0) is 0)
            {
                _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                _core.SetException(new OperationCanceledException(cancellationToken));
            }
        }

        public bool TryReset(TimeSpan delay, CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _state, 0, 1) is 2)
                return false;

            _core.Reset();

            if (!_timer.Change(delay, Timeout.InfiniteTimeSpan))
                ThrowTimerChangeFailed();

            _cancellationTokenRegistration = cancellationToken.UnsafeRegister(static (s, t) => Unsafe.As<DelayTaskSource>(s!).TrySetCanceled(t), this);

            if (Interlocked.CompareExchange(ref _state, 0, 0) is not 0)
                _cancellationTokenRegistration.Dispose();

            return true;
        }

        [DoesNotReturn]
        [StackTraceHidden]
        private static void ThrowTimerChangeFailed()
        {
            throw new InvalidOperationException("Failed to change timer.");
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
            _timer.Dispose();
            _cancellationTokenRegistration.Dispose();
        }
    }
}
