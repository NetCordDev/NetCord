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

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
    {
        Span<byte> bytes = new(new byte[Opus.FrameSize]);
        int r;
        fixed (byte* data = buffer, pcm = bytes)
            r = Opus.OpusDecode(_decoder, data, buffer.Length, (short*)pcm, Opus.FrameSamplesPerChannel, 0);

        _next.Write(bytes);
    }

    public override unsafe ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        Memory<byte> bytes = new(new byte[Opus.FrameSize]);
        fixed (byte* data = buffer.Span, pcm = bytes.Span)
            _ = Opus.OpusDecode(_decoder, data, buffer.Length, (short*)pcm, Opus.FrameSamplesPerChannel, 0);

        return _next.WriteAsync(bytes, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        _decoder.Dispose();
        base.Dispose(disposing);
    }
}
