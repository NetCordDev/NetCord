using System.Buffers;

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

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var len = buffer.Length;
        using var owner = MemoryPool<byte>.Shared.Rent(len);
        var data = owner.Memory;
        int count = Encode(buffer.Span, data.Span, len);
        await _next.WriteAsync(data[..count], cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        var len = buffer.Length;
        using var owner = MemoryPool<byte>.Shared.Rent(len);
        var data = owner.Memory.Span;
        int count = Encode(buffer, data, len);
        _next.Write(data[..count]);
    }

    private unsafe int Encode(ReadOnlySpan<byte> pcm, Span<byte> data, int len)
    {
        fixed (byte* pcmPtr = pcm, dataPtr = data)
            return Opus.OpusEncode(_encoder, (short*)pcmPtr, Opus.FrameSamplesPerChannel, dataPtr, len);
    }

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
