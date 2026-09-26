using System.Net;
using System.Net.Sockets;

namespace NetCord.Gateway.Voice.UdpSockets;

public sealed class UdpConnection(EndPoint endPoint) : IUdpConnection
{
    private readonly Socket _socket = new(SocketType.Dgram, ProtocolType.Udp);

    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        return _socket.ConnectAsync(endPoint, cancellationToken);
    }

    public void Abort()
    {
        _socket.Shutdown(SocketShutdown.Both);
    }

    public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _socket.ReceiveAsync(buffer, cancellationToken);
    }

    public void Send(ReadOnlySpan<byte> datagram)
    {
        _socket.Send(datagram);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default)
    {
        var task = _socket.SendAsync(datagram, cancellationToken);
        if (task.IsCompletedSuccessfully)
        {
            _ = task.Result;
            return default;
        }

        return new(task.AsTask());
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}
