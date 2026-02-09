using System.Diagnostics;

namespace NetCord.Gateway.Voice;

internal sealed class SpeedNormalizingStream(Stream next, float frameDuration) : RewritingStream(next)
{
    private readonly long _frameDurationTicks = (int)(frameDuration * 2) * Stopwatch.Frequency / TimeSpan.MillisecondsPerSecond / 2;

    private bool _used;
    private long _nextTick;

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_used)
        {
            var delayTicks = _nextTick - Stopwatch.GetTimestamp();
            var delay = (int)(delayTicks * TimeSpan.MillisecondsPerSecond / Stopwatch.Frequency);
            if (delay > 0)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _used = true;
            _nextTick = Stopwatch.GetTimestamp();
        }

        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _nextTick += _frameDurationTicks;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_used)
        {
            var delayTicks = _nextTick - Stopwatch.GetTimestamp();
            var delay = (int)(delayTicks * TimeSpan.MillisecondsPerSecond / Stopwatch.Frequency);
            if (delay > 0)
                Thread.Sleep(delay);
        }
        else
        {
            _used = true;
            _nextTick = Stopwatch.GetTimestamp();
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
}
