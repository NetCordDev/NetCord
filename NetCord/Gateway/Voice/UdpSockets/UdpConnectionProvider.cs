using System.Net;

namespace NetCord.Gateway.Voice.UdpSockets;

public class UdpConnectionProvider : IUdpConnectionProvider
{
    public static UdpConnectionProvider Instance { get; } = new();

    private UdpConnectionProvider()
    {
    }

    public IUdpConnection CreateConnection(string ip, ushort port)
    {
        return new UdpConnection(new IPEndPoint(IPAddress.Parse(ip), port));
    }
}
