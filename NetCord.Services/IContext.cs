namespace NetCord.Services;

public interface IContext
{
    public Guild? Guild { get; }
    public GatewayClient Client { get; }
}