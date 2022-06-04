using System.Net.Sockets;

namespace NetCord.Gateway.Voice.UdpSockets;

public class UdpSocket : IUdpSocket
{
    private UdpClient? _udpClient;

    public event Action<UdpReceiveResult>? DatagramReceive;

    public void Connect(string hostname, int port)
    {
        _udpClient = new(hostname, port);
        _ = ReadAsync();
    }

    public void Send(ReadOnlySpan<byte> datagram)
    {
        _udpClient!.Send(datagram);
    }

    public Task SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default) => _udpClient!.SendAsync(datagram, cancellationToken).AsTask();

    public void Dispose()
    {
        _udpClient!.Dispose();
    }

    public async Task ReadAsync()
    {
        var client = _udpClient;
        while (true)
        {
            try
            {
                var datagram = await client!.ReceiveAsync().ConfigureAwait(false);
                try
                {
                    DatagramReceive?.Invoke(datagram);
                }
                catch
                {
                }
            }
            catch
            {
                return;
            }
        }
    }
}
