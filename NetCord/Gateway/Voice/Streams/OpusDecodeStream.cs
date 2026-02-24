using System.Buffers;
using System.ComponentModel;

namespace NetCord.Gateway.Voice;

public sealed class OpusDecodeStream : RewritingStream
{
    private readonly OpusDecoder _decoder;
    private readonly Func<ReadOnlySpan<byte>, Span<byte>, int> _decode;
    private readonly PcmFormat _format;
    private readonly VoiceChannels _channels;
    private readonly int _frameSize;
    private readonly int _pcmBufferSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next">The stream that this stream is writing to.</param>
    /// <param name="format">The PCM format to decode to.</param>
    /// <param name="channels">Number of channels to decode.</param>
    /// <param name="leaveOpen">Whether to leave the next stream open when this stream is disposed.</param>
    public OpusDecodeStream(Stream next, PcmFormat format, VoiceChannels channels, bool leaveOpen = false) : base(next, leaveOpen)
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
        _pcmBufferSize = Opus.GetFrameBufferSize(samplesPerChannel, format, channels);
    }

    private int Decode(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        return _decoder.Decode(data, pcm, _frameSize, false);
    }

    private int DecodeFloat(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        return _decoder.DecodeFloat(data, pcm, _frameSize, false);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int size = _pcmBufferSize;

        var array = ArrayPool<byte>.Shared.Rent(size);

        try
        {
            int samplesPerChannel = _decode(buffer.Span, array.AsSpan(0, size));
            int written = Opus.GetFrameBufferSize(samplesPerChannel, _format, _channels);
            await _next.WriteAsync(array.AsMemory(0, written), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int size = _pcmBufferSize;

        var array = ArrayPool<byte>.Shared.Rent(size);

        try
        {
            int samplesPerChannel = _decode(buffer, array.AsSpan(0, size));
            int written = Opus.GetFrameBufferSize(samplesPerChannel, _format, _channels);
            _next.Write(array.AsSpan(0, written));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _decoder.Dispose();
        base.Dispose(disposing);
    }
}
