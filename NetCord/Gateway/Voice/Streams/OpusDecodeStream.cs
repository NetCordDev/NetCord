using System.Buffers;
using System.ComponentModel;

namespace NetCord.Gateway.Voice;

public class OpusDecodeStream : RewritingStream
{
    private readonly OpusDecoder _decoder;
    private readonly Func<ReadOnlySpan<byte>, Span<byte>, int> _decode;
    private readonly PcmFormat _format;
    private readonly VoiceChannels _channels;
    private readonly int _frameSize;
    private readonly int _bufferSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next">The stream that this stream is writing to.</param>
    /// <param name="format">The PCM format to decode to.</param>
    /// <param name="channels">Number of channels to decode.</param>
    public OpusDecodeStream(Stream next, PcmFormat format, VoiceChannels channels) : base(next)
    {
        _decoder = new(channels);
        _decode = format switch
        {
            PcmFormat.Short => Decode,
            PcmFormat.Float => DecodeFloat,
            _ => throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(PcmFormat))
        };
        _format = format;
        _channels = channels;
        int samplesPerChannel = _frameSize = Opus.GetSamplesPerChannel(Opus.MaxFrameDuration);
        _bufferSize = Opus.GetFrameBufferSize(samplesPerChannel, format, channels);
    }

    private int Decode(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        return _decoder.Decode(data, pcm, _frameSize);
    }

    private int DecodeFloat(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        return _decoder.DecodeFloat(data, pcm, _frameSize);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int bufferSize = _bufferSize;

        var array = ArrayPool<byte>.Shared.Rent(bufferSize);

        int samplesPerChannel = _decode(buffer.Span, array.AsSpan(0, bufferSize));
        int written = Opus.GetFrameBufferSize(samplesPerChannel, _format, _channels);
        await _next.WriteAsync(array.AsMemory(0, written), cancellationToken).ConfigureAwait(false);

        ArrayPool<byte>.Shared.Return(array);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int bufferSize = _bufferSize;

        var array = ArrayPool<byte>.Shared.Rent(bufferSize);

        int samplesPerChannel = _decode(buffer, array.AsSpan(0, bufferSize));
        int written = Opus.GetFrameBufferSize(samplesPerChannel, _format, _channels);
        _next.Write(array.AsSpan(0, written));

        ArrayPool<byte>.Shared.Return(array);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _decoder.Dispose();
        base.Dispose(disposing);
    }
}
