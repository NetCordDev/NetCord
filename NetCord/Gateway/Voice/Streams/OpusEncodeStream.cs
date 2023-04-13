using System.Buffers;

namespace NetCord.Gateway.Voice;

public class OpusEncodeStream : RewritingStream
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="next">The stream that this stream is writing to.</param>
    /// <param name="channels">Number of channels in input signal.</param>
    /// <param name="application">Opus coding mode.</param>
    /// <param name="segment">Whether to segment the written data into Opus frames. You can set this to <see langword="false"/> if you are sure to write exactly one Opus frame at a time.</param>
    public OpusEncodeStream(Stream next, VoiceChannels channels, OpusApplication application, bool segment = true) : base(segment ? new SegmentingStream(new OpusEncodeStreamInternal(next, channels, application), channels)
                                                                                                                                  : new OpusEncodeStreamInternal(next, channels, application))
    {
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _next.WriteAsync(buffer, cancellationToken);

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _next.Write(buffer);
    }

    private class SegmentingStream : RewritingStream
    {
        private int _offset;
        private readonly int _frameSize;
        private readonly Memory<byte> _buffer;

        public SegmentingStream(Stream next, VoiceChannels channels) : base(next)
        {
            _buffer = new byte[_frameSize = Opus.GetFrameSize(channels)];
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_offset == 0)
                await WriteAsyncInternal().ConfigureAwait(false);
            else
            {
                var end = _frameSize - _offset;
                if (buffer.Length > end)
                {
                    buffer[..end].CopyTo(_buffer[_offset..]);
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
                while (buffer.Length > _frameSize)
                {
                    await _next.WriteAsync(buffer[.._frameSize], cancellationToken).ConfigureAwait(false);
                    buffer = buffer[_frameSize..];
                }
                buffer.CopyTo(_buffer);
                _offset = buffer.Length;
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            var buf = _buffer.Span;
            if (_offset == 0)
                WriteInternal(buffer, buf);
            else
            {
                var end = _frameSize - _offset;
                if (buffer.Length > end)
                {
                    buffer[..end].CopyTo(buf[_offset..]);
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
                while (buffer.Length > _frameSize)
                {
                    _next.Write(buffer[.._frameSize]);
                    buffer = buffer[_frameSize..];
                }
                buffer.CopyTo(buf);
                _offset = buffer.Length;
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

    private class OpusEncodeStreamInternal : RewritingStream
    {
        private readonly OpusEncoder _encoder;

        public OpusEncodeStreamInternal(Stream next, VoiceChannels channels, OpusApplication application) : base(next)
        {
            _encoder = new(channels, application);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            using var owner = MemoryPool<byte>.Shared.Rent(Opus.MaxOpusFrameLength);
            var data = owner.Memory;
            int count = _encoder.Encode(buffer.Span, data.Span);
            await _next.WriteAsync(data[..count], cancellationToken).ConfigureAwait(false);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            using var owner = MemoryPool<byte>.Shared.Rent(Opus.MaxOpusFrameLength);
            var data = owner.Memory.Span;
            int count = _encoder.Encode(buffer, data);
            _next.Write(data[..count]);
        }

        protected override void Dispose(bool disposing)
        {
            _encoder.Dispose();
            base.Dispose(disposing);
        }
    }
}
