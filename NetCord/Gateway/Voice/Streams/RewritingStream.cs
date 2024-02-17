namespace NetCord.Gateway.Voice;

/// <summary>
/// 
/// </summary>
/// <param name="next">The stream that this stream is writing to.</param>
public abstract class RewritingStream(Stream next) : Stream
{
    private protected readonly Stream _next = next;

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
        _next.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
        => _next.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(new ReadOnlySpan<byte>(buffer, offset, count));
    }

    public abstract override void Write(ReadOnlySpan<byte> buffer);

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();

    public abstract override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _next.Dispose();
    }
}
