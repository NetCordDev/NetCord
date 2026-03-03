using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace NetCord.Gateway.Voice;

internal sealed class SpeedNormalizingStream : RewritingStream
{
    private readonly long _timestampFrequency;
    private readonly long _frameDurationTicks;
    private readonly DelayTaskSource _delaySource;
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
                delaySource.Reset(new(delay), cancellationToken);
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
            if (Interlocked.Exchange(ref _state, 1) is 0)
            {
                _cancellationTokenRegistration.Dispose();
                _core.SetResult(true);
            }
        }

        private void TrySetException(Exception exception)
        {
            if (Interlocked.Exchange(ref _state, 1) is 0)
            {
                _cancellationTokenRegistration.Dispose();
                _core.SetException(exception);
            }
        }

        public void Reset(TimeSpan delay, CancellationToken cancellationToken)
        {
            _core.Reset();
            _state = 0;
            _cancellationTokenRegistration = cancellationToken.UnsafeRegister(static (s, t) => Unsafe.As<DelayTaskSource>(s!).TrySetException(new OperationCanceledException(t)), this);

            if (_state is not 0)
            {
                _cancellationTokenRegistration.Dispose();
                return;
            }
            
            if (!_timer.Change(delay, Timeout.InfiniteTimeSpan))
                ThrowTimerChangeFailed();
        }

        [DoesNotReturn]
        [StackTraceHidden]
        private static void ThrowTimerChangeFailed()
        {
            throw new InvalidOperationException("Failed to change timer.");
        }

        public void GetResult(short token)
        {
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
