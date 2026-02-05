using System.Buffers;
using System.ComponentModel;

namespace NetCord.Gateway.Voice;

/// <summary>
/// 
/// </summary>
/// <param name="next">The stream that this stream is writing to.</param>
/// <param name="frameDuration">The duration of each Opus frame, in milliseconds.</param>
/// <param name="format">The PCM format to encode from.</param>
/// <param name="channels">Number of channels in input signal.</param>
/// <param name="application">Opus coding mode.</param>
/// <param name="segment">Whether to segment the written data into Opus frames. You can set this to <see langword="false"/> if you are sure to write exactly one Opus frame at a time.</param>
public class OpusEncodeStream(Stream next, PcmFormat format, VoiceChannels channels, OpusApplication application, float frameDuration = 20, bool segment = true) : RewritingStream(CreateNextStream(next, frameDuration, format, channels, application, segment))
{
    private static Stream CreateNextStream(Stream next, float frameDuration, PcmFormat format, VoiceChannels channels, OpusApplication application, bool segment)
    {
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

    private class SegmentingStream : RewritingStream
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
            if (_offset == 0)
                await WriteAsyncInternal().ConfigureAwait(false);
            else
            {
                var end = _bufferSize - _offset;
                if (buffer.Length > end)
                {
                    buffer[..end].CopyTo(_buffer[_offset..]);
                    await _next.WriteAsync(_buffer, cancellationToken).ConfigureAwait(false);
                    _offset = 0;
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
            if (_offset == 0)
                WriteInternal(buffer, buf);
            else
            {
                var end = _bufferSize - _offset;
                if (buffer.Length > end)
                {
                    buffer[..end].CopyTo(buf[_offset..]);
                    _next.Write(buf);
                    _offset = 0;
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

    private partial class OpusEncodeStreamInternal : RewritingStream
    {
        private readonly OpusEncoder _encoder;
        private readonly Func<ReadOnlySpan<byte>, Span<byte>, int> _encode;
        private readonly int _frameSize;
        private readonly int _bufferSize;

        public OpusEncodeStreamInternal(Stream next, float frameDuration, PcmFormat format, VoiceChannels channels, OpusApplication application) : base(next)
        {
            _encoder = new(channels, application);
            _encode = format switch
            {
                PcmFormat.Short => Encode,
                PcmFormat.Float => EncodeFloat,
                _ => throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(PcmFormat)),
            };
            _frameSize = Opus.GetSamplesPerChannel(frameDuration);
            _bufferSize = Opus.GetMaxOpusFrameSize(frameDuration);
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
            var array = ArrayPool<byte>.Shared.Rent(_bufferSize);

            int count = _encode(buffer.Span, array.AsSpan());
            await _next.WriteAsync(array.AsMemory(0, count), cancellationToken).ConfigureAwait(false);

            ArrayPool<byte>.Shared.Return(array);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            var array = ArrayPool<byte>.Shared.Rent(_bufferSize);

            var data = array.AsSpan();
            int count = _encode(buffer, data);
            _next.Write(data[..count]);

            ArrayPool<byte>.Shared.Return(array);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _encoder.Dispose();
            base.Dispose(disposing);
        }
    }
}
