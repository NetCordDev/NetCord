using System.Diagnostics;

namespace NetCord.Gateway.Voice;

internal class SpeedNormalizingStream(Stream next, float frameDuration) : RewritingStream(next)
{
    private readonly long _frameDurationTicks = (int)(frameDuration * 2) * Stopwatch.Frequency / TimeSpan.MillisecondsPerSecond / 2;

    private bool _used;
    private long _startTick;

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_used)
        {
            var delay = _startTick - Stopwatch.GetTimestamp();
            if (delay > 0)
                await Task.Delay((int)(delay * TimeSpan.MillisecondsPerSecond / Stopwatch.Frequency), cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _used = true;
            _startTick = Stopwatch.GetTimestamp();
        }

        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _startTick += _frameDurationTicks;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_used)
        {
            var delay = _startTick - Stopwatch.GetTimestamp();
            if (delay > 0)
                Thread.Sleep((int)(delay * TimeSpan.MillisecondsPerSecond / Stopwatch.Frequency));
        }
        else
        {
            _used = true;
            _startTick = Stopwatch.GetTimestamp();
        }

        _next.Write(buffer);
        _startTick += _frameDurationTicks;
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
