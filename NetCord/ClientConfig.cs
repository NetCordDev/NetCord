namespace NetCord;

public class ClientConfig
{
    public GatewayIntent Intents { get; init; } = GatewayIntent.AllNonPrivileged;
}