namespace NetCord.Gateway.Voice;

internal class SpeedNormalizingStream(Stream next) : RewritingStream(next)
{
    private bool _used;
    private int _startTick;

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_used)
        {
            var delay = _startTick - Environment.TickCount;
            if (delay > 0)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            _used = true;
            _startTick = Environment.TickCount;
        }
        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _startTick += Opus.FrameDuration;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_used)
        {
            var delay = _startTick - Environment.TickCount;
            if (delay > 0)
                Thread.Sleep(delay);
        }
        else
        {
            _used = true;
            _startTick = Environment.TickCount;
        }
        _next.Write(buffer);
        _startTick += Opus.FrameDuration;
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
