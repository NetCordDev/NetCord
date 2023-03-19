using System.Net.Sockets;

namespace NetCord.Gateway.Voice.UdpSockets;

public class UdpSocket : IUdpSocket
{
    private UdpClient? _udpClient;

    public event Action<UdpReceiveResult>? DatagramReceive;

    public void Connect(string hostname, int port)
    {
        _udpClient?.Dispose();
        _udpClient = new(hostname, port);
        _ = ReadAsync();
    }

    public void Send(ReadOnlySpan<byte> datagram)
    {
        _udpClient!.Send(datagram);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default)
    {
        var task = _udpClient!.SendAsync(datagram, cancellationToken);
        if (task.IsCompletedSuccessfully)
        {
            _ = task.Result;
            return default;
        }

        return new(task.AsTask());
    }

    public void Dispose()
    {
        _udpClient?.Dispose();
    }

    private async Task ReadAsync()
    {
        var client = _udpClient!;
        while (true)
        {
            try
            {
                var datagram = await client.ReceiveAsync().ConfigureAwait(false);
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
