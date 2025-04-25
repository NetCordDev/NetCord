using NetCord.Gateway;

namespace NetCord.Services;

public interface IGatewayClientContext : IContext
{
    public GatewayClient Client { get; }
}
