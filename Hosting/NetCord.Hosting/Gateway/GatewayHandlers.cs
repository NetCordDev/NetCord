using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public interface IGatewayHandler;

internal interface IDelegateGatewayHandlerBase : IGatewayHandler
{
    internal string Name { get; }
}

internal interface IDelegateGatewayHandler : IDelegateGatewayHandlerBase
{
    public ValueTask HandleAsync();
}

internal interface IDelegateGatewayHandler<T> : IDelegateGatewayHandlerBase
{
    public ValueTask HandleAsync(T arg);
}

internal class DelegateGatewayHandler(string name, IServiceProvider services, Delegate handler) : IDelegateGatewayHandler
{
    private readonly Func<IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<IServiceProvider, ValueTask>>(handler, []);

    string IDelegateGatewayHandlerBase.Name => name;

    public ValueTask HandleAsync() => _handler(services);
}

internal class DelegateGatewayHandler<T>(string name, IServiceProvider services, Delegate handler) : IDelegateGatewayHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    string IDelegateGatewayHandlerBase.Name => name;

    public ValueTask HandleAsync(T arg) => _handler(arg, services);
}

public interface IShardedGatewayHandler;

internal interface IDelegateShardedGatewayHandlerBase : IShardedGatewayHandler
{
    internal string Name { get; }
}

internal interface IDelegateShardedGatewayHandler : IDelegateShardedGatewayHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client);
}

internal interface IDelegateShardedGatewayHandler<T> : IDelegateShardedGatewayHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client, T arg);
}

internal class DelegateShardedGatewayHandler(string name, IServiceProvider services, Delegate handler) : IDelegateShardedGatewayHandler
{
    private readonly Func<GatewayClient, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient)]);

    string IDelegateShardedGatewayHandlerBase.Name => name;

    public ValueTask HandleAsync(GatewayClient client) => _handler(client, services);
}

internal class DelegateShardedGatewayHandler<T>(string name, IServiceProvider services, Delegate handler) : IDelegateShardedGatewayHandler<T>
{
    private readonly Func<GatewayClient, T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, T, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient), typeof(T)]);

    string IDelegateShardedGatewayHandlerBase.Name => name;

    public ValueTask HandleAsync(GatewayClient client, T arg) => _handler(client, arg, services);
}
