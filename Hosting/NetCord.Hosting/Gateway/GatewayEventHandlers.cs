using System.Reflection;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public interface IGatewayEventHandlerBase
{
    public IEnumerable<string> GetEvents()
    {
        var type = GetType();
        var attributes = type.GetCustomAttributes<GatewayEventAttribute>();
        return attributes.Select(a => a.Name);
    }
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
    public IEnumerable<string> GetEvents()
    {
        var type = GetType();
        var attributes = type.GetCustomAttributes<GatewayEventAttribute>();
        return attributes.Select(a => a.Name);
    }
}

public interface IShardedGatewayEventHandler : IShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client);
}

public interface IShardedGatewayEventHandler<T> : IShardedGatewayEventHandlerBase
{
    public ValueTask HandleAsync(GatewayClient client, T arg);
}

internal class DelegateGatewayEventHandler(string name, IServiceProvider services, Delegate handler) : IGatewayEventHandler
{
    private readonly Func<IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<IServiceProvider, ValueTask>>(handler, []);

    public ValueTask HandleAsync() => _handler(services);

    public IEnumerable<string> GetEvents()
    {
        yield return name;
    }
}

internal class DelegateGatewayEventHandler<T>(string name, IServiceProvider services, Delegate handler) : IGatewayEventHandler<T>
{
    private readonly Func<T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]);

    public ValueTask HandleAsync(T arg) => _handler(arg, services);

    public IEnumerable<string> GetEvents()
    {
        yield return name;
    }
}

internal class DelegateShardedGatewayEventHandler(string name, IServiceProvider services, Delegate handler) : IShardedGatewayEventHandler
{
    private readonly Func<GatewayClient, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient)]);

    public ValueTask HandleAsync(GatewayClient client) => _handler(client, services);

    public IEnumerable<string> GetEvents()
    {
        yield return name;
    }
}

internal class DelegateShardedGatewayEventHandler<T>(string name, IServiceProvider services, Delegate handler) : IShardedGatewayEventHandler<T>
{
    private readonly Func<GatewayClient, T, IServiceProvider, ValueTask> _handler = DelegateHandlerHelper.CreateHandler<Func<GatewayClient, T, IServiceProvider, ValueTask>>(handler, [typeof(GatewayClient), typeof(T)]);

    public ValueTask HandleAsync(GatewayClient client, T arg) => _handler(client, arg, services);

    public IEnumerable<string> GetEvents()
    {
        yield return name;
    }
}
