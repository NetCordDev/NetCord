using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public interface IGatewayEventHandler;

internal interface IDelegateGatewayEventHandlerBase : IGatewayEventHandler
{
    internal string Name { get; }
}

internal interface IDelegateGatewayEventHandler : IDelegateGatewayEventHandlerBase
{
    public ValueTask HandleAsync();
}

internal interface IDelegateGatewayEventHandler<T> : IDelegateGatewayEventHandlerBase
{
    public ValueTask HandleAsync(T arg);
}

internal class DelegateGatewayEventHandler(string name, IServiceProvider services, Delegate handler) : IDelegateGatewayEventHandler
{
    private readonly Func<IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<IServiceProvider, ValueTask>>(handler, []);

    string IDelegateGatewayEventHandlerBase.Name => name;

    public ValueTask HandleAsync() => _handler(services);
}

internal class DelegateGatewayEventHandler<T>(string name, IServiceProvider services, Delegate handler) : IDelegateGatewayEventHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    string IDelegateGatewayEventHandlerBase.Name => name;

    public ValueTask HandleAsync(T arg) => _handler(arg, services);
}

public interface IShardedGatewayEventHandler;

internal interface IDelegateShardedGatewayEventHandlerBase : IShardedGatewayEventHandler
{
    internal string Name { get; }
}

internal interface IDelegateShardedGatewayEventHandler : IDelegateShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client);
}

internal interface IDelegateShardedGatewayEventHandler<T> : IDelegateShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client, T arg);
}

internal class DelegateShardedGatewayEventHandler(string name, IServiceProvider services, Delegate handler) : IDelegateShardedGatewayEventHandler
{
    private readonly Func<GatewayClient, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient)]);

    string IDelegateShardedGatewayEventHandlerBase.Name => name;

    public ValueTask HandleAsync(GatewayClient client) => _handler(client, services);
}

internal class DelegateShardedGatewayEventHandler<T>(string name, IServiceProvider services, Delegate handler) : IDelegateShardedGatewayEventHandler<T>
{
    private readonly Func<GatewayClient, T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, T, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient), typeof(T)]);

    string IDelegateShardedGatewayEventHandlerBase.Name => name;

    public ValueTask HandleAsync(GatewayClient client, T arg) => _handler(client, arg, services);
}
