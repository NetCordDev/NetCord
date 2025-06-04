namespace NetCord.Gateway.Voice.UdpSockets;

public interface IUdpConnection : IDisposable
{
    public ValueTask OpenAsync(CancellationToken cancellationToken = default);

    public ValueTask SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default);

    public void Send(ReadOnlySpan<byte> datagram);

    public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

    public void Abort();
}
