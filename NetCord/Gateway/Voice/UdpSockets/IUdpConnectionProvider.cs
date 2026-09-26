namespace NetCord.Gateway.Voice.UdpSockets;

public interface IUdpConnectionProvider
{
    public IUdpConnection CreateConnection(string ip, ushort port);
}
