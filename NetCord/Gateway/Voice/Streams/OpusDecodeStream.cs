using System.Buffers;

namespace NetCord.Gateway.Voice.Streams;

internal class OpusDecodeStream : RewritingStream
{
    private readonly OpusDecoderHandle _decoder;

    public OpusDecodeStream(Stream next) : base(next)
    {
        _decoder = Opus.OpusDecoderCreate(Opus.SamplingRate, Opus.Channels, out var error);
        if (error != 0)
            throw new OpusException(error);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(Opus.FrameSize);
        var pcm = owner.Memory[..Opus.FrameSize];
        Decode(buffer.Span, pcm.Span);
        await _next.WriteAsync(pcm, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        using var owner = MemoryPool<byte>.Shared.Rent(Opus.FrameSize);
        var pcm = owner.Memory.Span[..Opus.FrameSize];
        Decode(buffer, pcm);
        _next.Write(pcm);
    }

    private unsafe void Decode(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        fixed (byte* dataPtr = data, pcmPtr = pcm)
            _ = Opus.OpusDecode(_decoder, dataPtr, data.Length, (short*)pcmPtr, Opus.FrameSamplesPerChannel, 0);
    }

    protected override void Dispose(bool disposing)
    {
        _decoder.Dispose();
        base.Dispose(disposing);
    }
}
