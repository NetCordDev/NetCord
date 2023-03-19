using System.Net.Sockets;

namespace NetCord.Gateway.Voice.UdpSockets;

public interface IUdpSocket : IDisposable
{
    public event Action<UdpReceiveResult>? DatagramReceive;

    public ValueTask SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default);

    public void Send(ReadOnlySpan<byte> datagram);

    public void Connect(string hostname, int port);
}
