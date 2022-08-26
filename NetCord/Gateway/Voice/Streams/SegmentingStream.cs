namespace NetCord.Gateway.Voice.Streams;

internal class SegmentingStream : RewritingStream
{
    private int _offset;
    private readonly Memory<byte> _buffer = new(new byte[Opus.FrameSize]);

    public SegmentingStream(Stream next) : base(next)
    {
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_offset == 0)
            await WriteAsyncInternal().ConfigureAwait(false);
        else
        {
            var end = Opus.FrameSize - _offset;
            if (buffer.Length > end)
            {
                buffer[..end].CopyTo(_buffer[_offset..]);
                _offset = 0;
                await _next.WriteAsync(_buffer, cancellationToken).ConfigureAwait(false);
                buffer = buffer[end..];
                await WriteAsyncInternal().ConfigureAwait(false);
            }
            else
            {
                buffer.CopyTo(_buffer[_offset..]);
                _offset += buffer.Length;
            }
        }

        async ValueTask WriteAsyncInternal()
        {
            while (true)
            {
                if (buffer.Length > Opus.FrameSize)
                {
                    await _next.WriteAsync(buffer[..Opus.FrameSize], cancellationToken).ConfigureAwait(false);
                    buffer = buffer[Opus.FrameSize..];
                }
                else
                {
                    buffer.CopyTo(_buffer);
                    _offset = buffer.Length;
                    break;
                }
            }
        }
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        var buf = _buffer.Span;
        if (_offset == 0)
            WriteInternal(buffer, buf);
        else
        {
            var end = Opus.FrameSize - _offset;
            if (buffer.Length > end)
            {
                buffer[..end].CopyTo(buf[_offset..]);
                _offset = 0;
                _next.Write(buf);
                buffer = buffer[end..];
                WriteInternal(buffer, buf);
            }
            else
            {
                buffer.CopyTo(buf[_offset..]);
                _offset += buffer.Length;
            }
        }

        void WriteInternal(ReadOnlySpan<byte> buffer, Span<byte> buf)
        {
            while (true)
            {
                if (buffer.Length > Opus.FrameSize)
                {
                    _next.Write(buffer[..Opus.FrameSize]);
                    buffer = buffer[Opus.FrameSize..];
                }
                else
                {
                    buffer.CopyTo(buf);
                    _offset = buffer.Length;
                    break;
                }
            }
        }
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        if (_offset != 0)
        {
            await _next.WriteAsync(_buffer[.._offset], cancellationToken).ConfigureAwait(false);
            _offset = 0;
        }
        await base.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Flush()
    {
        if (_offset != 0)
        {
            _next.Write(_buffer[.._offset].Span);
            _offset = 0;
        }
        base.Flush();
    }
}