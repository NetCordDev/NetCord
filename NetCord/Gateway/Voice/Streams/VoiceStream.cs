using NetCord.Gateway.Voice.UdpSockets;

namespace NetCord.Gateway.Voice.Streams;

internal class VoiceStream : Stream
{
    private readonly IUdpSocket _udpSocket;

    public VoiceStream(IUdpSocket udpSocket)
    {
        _udpSocket = udpSocket;
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await _udpSocket.SendAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(byte[] buffer, int offset, int count) => Write(new ReadOnlySpan<byte>(buffer, offset, count));

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _udpSocket.Send(buffer);
    }
}