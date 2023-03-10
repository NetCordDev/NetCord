using NetCord.Gateway;

namespace NetCord.Services;

public interface IContext
{
    public GatewayClient Client { get; }

    public IServiceProvider? Services { get; }
}
