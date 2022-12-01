namespace NetCord.Gateway.Voice.Streams;

internal class SpeedNormalizingStream : RewritingStream
{
    private bool _new = true;
    private int _startTick;

    public SpeedNormalizingStream(Stream next) : base(next)
    {
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_new)
        {
            _new = false;
            _startTick = Environment.TickCount;
        }
        else
        {
            var delay = _startTick - Environment.TickCount;
            if (delay > 0)
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        await _next.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        _startTick += Opus.FrameDuration;
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (_new)
        {
            _new = false;
            _startTick = Environment.TickCount;
        }
        else
        {
            var delay = _startTick - Environment.TickCount;
            if (delay > 0)
                Thread.Sleep(delay);
        }
        _next.Write(buffer);
        _startTick += Opus.FrameDuration;
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        _new = true;
        return base.FlushAsync(cancellationToken);
    }

    public override void Flush()
    {
        _new = true;
        base.Flush();
    }
}
