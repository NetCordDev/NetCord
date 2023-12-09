using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public interface IGatewayEventHandlerBase
{
}

public interface IGatewayEventHandler : IGatewayEventHandlerBase
{
    public ValueTask HandleAsync();
}

public interface IGatewayEventHandler<T> : IGatewayEventHandlerBase
{
    public ValueTask HandleAsync(T arg);
}

public interface IShardedGatewayEventHandlerBase
{
}

public interface IShardedGatewayEventHandler : IShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client);
}

public interface IShardedGatewayEventHandler<T> : IShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client, T arg);
}
