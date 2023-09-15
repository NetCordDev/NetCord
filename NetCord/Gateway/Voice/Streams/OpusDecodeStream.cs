using System.Buffers;
using System.ComponentModel;

namespace NetCord.Gateway.Voice;

public unsafe partial class OpusDecodeStream : RewritingStream
{
    private readonly OpusDecoder _decoder;
    private readonly delegate*<OpusDecoder, ReadOnlySpan<byte>, Span<byte>, void> _decode;
    private readonly int _frameSize;

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
            PcmFormat.Short => &Decode,
            PcmFormat.Float => &DecodeFloat,
            _ => throw new InvalidEnumArgumentException(nameof(format), (int)format, typeof(PcmFormat))
        };
        _frameSize = Opus.GetFrameSize(format, channels);

        static void Decode(OpusDecoder decoder, ReadOnlySpan<byte> pcm, Span<byte> data)
        {
            decoder.Decode(pcm, data);
        }

        static void DecodeFloat(OpusDecoder decoder, ReadOnlySpan<byte> pcm, Span<byte> data)
        {
            decoder.DecodeFloat(pcm, data);
        }
    }

    private void Decode(ReadOnlySpan<byte> pcm, Span<byte> data)
    {
        _decode(_decoder, pcm, data);
    }
}

public partial class OpusDecodeStream
{
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(_frameSize);
        var pcm = owner.Memory[.._frameSize];
        Decode(buffer.Span, pcm.Span);
        await _next.WriteAsync(pcm, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(_frameSize);
        var pcm = owner.Memory.Span[.._frameSize];
        Decode(buffer, pcm);
        _next.Write(pcm);
    }

    protected override void Dispose(bool disposing)
    {
        _decoder.Dispose();
        base.Dispose(disposing);
    }
}
