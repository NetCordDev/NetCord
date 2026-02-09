using System.Buffers;
using System.ComponentModel;

namespace NetCord.Gateway.Voice;

/// <summary>
/// 
/// </summary>
/// <param name="next">The stream that this stream is writing to.</param>
/// <param name="format">The PCM format to encode from.</param>
/// <param name="channels">Number of channels in input signal.</param>
/// <param name="application">Opus coding mode.</param>
/// <param name="configuration">The configuration of the stream.</param>
public sealed class OpusEncodeStream(Stream next, PcmFormat format, VoiceChannels channels, OpusApplication application, OpusEncodeStreamConfiguration? configuration = null) : RewritingStream(CreateNextStream(next, format, channels, application, configuration))
{
    private static Stream CreateNextStream(Stream next, PcmFormat format, VoiceChannels channels, OpusApplication application, OpusEncodeStreamConfiguration? configuration)
    {
        var frameDuration = configuration?.FrameDuration ?? Opus.DefaultFrameDuration;
        var segment = configuration?.Segment ?? true;

        OpusEncodeStreamInternal encodeStream = new(next, frameDuration, format, channels, application);
        return segment ? new SegmentingStream(encodeStream, frameDuration, format, channels)
                       : encodeStream;
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _next.WriteAsync(buffer, cancellationToken);

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _next.Write(buffer);
    }

    private sealed class SegmentingStream : RewritingStream
    {
        private int _offset;
        private readonly int _bufferSize;
        private readonly Memory<byte> _buffer;

        public SegmentingStream(Stream next, float frameDuration, PcmFormat format, VoiceChannels channels) : base(next)
        {
            int samplesPerChannel = Opus.GetSamplesPerChannel(frameDuration);
            int bufferSize = _bufferSize = Opus.GetFrameBufferSize(samplesPerChannel, format, channels);
            _buffer = GC.AllocateUninitializedArray<byte>(bufferSize);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_offset is 0)
                await WriteAsyncInternal().ConfigureAwait(false);
            else
            {
                var end = _bufferSize - _offset;
                if (buffer.Length >= end)
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
                while (buffer.Length >= _bufferSize)
                {
                    await _next.WriteAsync(buffer[.._bufferSize], cancellationToken).ConfigureAwait(false);
                    buffer = buffer[_bufferSize..];
                }
                buffer.CopyTo(_buffer);
                _offset = buffer.Length;
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            var buf = _buffer.Span;
            if (_offset is 0)
                WriteInternal(buffer, buf);
            else
            {
                var end = _bufferSize - _offset;
                if (buffer.Length >= end)
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
                while (buffer.Length >= _bufferSize)
                {
                    _next.Write(buffer[.._bufferSize]);
                    buffer = buffer[_bufferSize..];
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

    private sealed partial class OpusEncodeStreamInternal : RewritingStream
    {
        private readonly OpusEncoder _encoder;
        private readonly Func<ReadOnlySpan<byte>, Span<byte>, int> _encode;
        private readonly int _frameSize;
        private readonly int _opusBufferSize;
        private readonly int _pcmBufferSize;

        public OpusEncodeStreamInternal(Stream next, float frameDuration, PcmFormat format, VoiceChannels channels, OpusApplication application) : base(next)
        {
            _encoder = new(channels, application);
            _encode = format switch
            {
                PcmFormat.Short => Encode,
                PcmFormat.Float => EncodeFloat,
                _ => throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(PcmFormat)),
            };
            int samplesPerChannel = _frameSize = Opus.GetSamplesPerChannel(frameDuration);
            _opusBufferSize = Opus.GetMaxOpusFrameSize(frameDuration);
            _pcmBufferSize = Opus.GetFrameBufferSize(samplesPerChannel, format, channels);
        }

        private int Encode(ReadOnlySpan<byte> pcm, Span<byte> data)
        {
            return _encoder.Encode(pcm, _frameSize, data);
        }

        private int EncodeFloat(ReadOnlySpan<byte> pcm, Span<byte> data)
        {
            return _encoder.EncodeFloat(pcm, _frameSize, data);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int size = _opusBufferSize;

            var array = ArrayPool<byte>.Shared.Rent(size);

            try
            {
                int count = _encode(buffer.Span, array.AsSpan(0, size));
                await _next.WriteAsync(array.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            int size = _opusBufferSize;

            var array = ArrayPool<byte>.Shared.Rent(size);

            try
            {
                var data = array.AsSpan(0, size);
                int count = _encode(buffer, data);
                _next.Write(data[..count]);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            int size = _pcmBufferSize;

            var silenceArray = ArrayPool<byte>.Shared.Rent(size);

            try
            {
                silenceArray.AsSpan(0, size).Clear();

                var silenceBuffer = silenceArray.AsMemory(0, size);

                for (int i = 0; i < 5; i++)
                    await WriteAsync(silenceBuffer, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(silenceArray);
            }

            await base.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public override void Flush()
        {
            int size = _pcmBufferSize;

            var silenceArray = ArrayPool<byte>.Shared.Rent(size);

            try
            {
                var silenceBuffer = silenceArray.AsSpan(0, size);

                silenceBuffer.Clear();

                for (int i = 0; i < 5; i++)
                    Write(silenceBuffer);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(silenceArray);
            }

            base.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _encoder.Dispose();
            base.Dispose(disposing);
        }
    }
}
