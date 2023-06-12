using NetCord.Gateway;

namespace NetCord.Services;

public interface IGatewayClientContext
{
    public GatewayClient Client { get; }
}
