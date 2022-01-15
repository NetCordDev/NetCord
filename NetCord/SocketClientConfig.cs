namespace NetCord;

public class SocketClientConfig
{
    public GatewayIntent Intents { get; init; } = GatewayIntent.AllNonPrivileged;
}