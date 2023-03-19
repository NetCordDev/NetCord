using System.Buffers;

namespace NetCord.Gateway.Voice;

public class OpusDecodeStream : RewritingStream
{
    private readonly OpusDecoder _decoder;
    private readonly int _frameSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next">The stream that this stream is writing to.</param>
    /// <param name="channels">Number of channels to decode.</param>
    public OpusDecodeStream(Stream next, VoiceChannels channels) : base(next)
    {
        _decoder = new(channels);
        _frameSize = Opus.GetFrameSize(channels);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(_frameSize);
        var pcm = owner.Memory[.._frameSize];
        _decoder.Decode(buffer.Span, pcm.Span);
        await _next.WriteAsync(pcm, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(_frameSize);
        var pcm = owner.Memory.Span[.._frameSize];
        _decoder.Decode(buffer, pcm);
        _next.Write(pcm);
    }

    protected override void Dispose(bool disposing)
    {
        _decoder.Dispose();
        base.Dispose(disposing);
    }
}
