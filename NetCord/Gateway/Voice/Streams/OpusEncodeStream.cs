namespace NetCord.Gateway.Voice.Streams;

internal class OpusEncodeStream : RewritingStream
{
    private readonly OpusEncoderHandle _encoder;

    public OpusEncodeStream(Stream next, OpusApplication application) : base(next)
    {
        _encoder = Opus.OpusEncoderCreate(Opus.SamplingRate, Opus.Channels, application, out var error);
        if (error != 0)
            throw new OpusException(error);
    }

    public override unsafe ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var len = buffer.Length;
        Memory<byte> data = new byte[len];

        int count;
        fixed (byte* pcmPtr = buffer.Span, dataPtr = data.Span)
            count = OpusEncode(len, pcmPtr, dataPtr);

        return _next.WriteAsync(data[..count], cancellationToken);
    }

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
    {
        var len = buffer.Length;
        Span<byte> data = new byte[len];

        int count;
        fixed (byte* pcmPtr = buffer, dataPtr = data)
            count = OpusEncode(len, pcmPtr, dataPtr);

        _next.Write(data[..count]);
    }

    private unsafe int OpusEncode(int len, byte* pcmPtr, byte* dataPtr) => Opus.OpusEncode(_encoder, (short*)pcmPtr, Opus.FrameSamplesPerChannel, dataPtr, len);

    protected override void Dispose(bool disposing)
    {
        _encoder.Dispose();
        base.Dispose(disposing);
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _next.WriteAsync(new byte[]
        {
            0xF8,
            0xFF,
            0xFE,
        }, cancellationToken).ConfigureAwait(false);
        await base.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Flush()
    {
        _next.Write(new byte[]
        {
            0xF8,
            0xFF,
            0xFE,
        });
        base.Flush();
    }
}