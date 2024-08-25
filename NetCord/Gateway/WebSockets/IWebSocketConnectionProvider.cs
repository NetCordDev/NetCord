namespace NetCord.Gateway.WebSockets;

public interface IWebSocketConnectionProvider
{
    public IWebSocketConnection CreateConnection();
}
