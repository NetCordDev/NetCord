using System.Reflection;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public interface IGatewayEventHandlerBase
{
    /// <summary>
    /// Gets the names of the events this handler listens for.
    /// </summary>
    /// <returns>The names of the events this handler listens for.</returns>
    public IEnumerable<string> GetEvents()
    {
        var type = GetType();
        var attributes = type.GetCustomAttributes<GatewayEventAttribute>();
        return attributes.Select(a => a.Name);
    }
}

public interface IGatewayEventHandler : IGatewayEventHandlerBase
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync();
}

public interface IGatewayEventHandler<T> : IGatewayEventHandlerBase
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="arg">The event argument.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(T arg);
}

public interface IShardedGatewayEventHandlerBase
{
    /// <summary>
    /// Gets the names of the events this handler listens for.
    /// </summary>
    /// <returns>The names of the events this handler listens for.</returns>
    public IEnumerable<string> GetEvents()
    {
        var type = GetType();
        var attributes = type.GetCustomAttributes<GatewayEventAttribute>();
        return attributes.Select(a => a.Name);
    }
}

public interface IShardedGatewayEventHandler : IShardedGatewayEventHandlerBase
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="client">The <see cref="GatewayClient"/> that represents the shard that received the event.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(GatewayClient client);
}

public interface IShardedGatewayEventHandler<T> : IShardedGatewayEventHandlerBase
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="client">The <see cref="GatewayClient"/> that represents the shard that received the event.</param>
    /// <param name="arg">The event argument.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
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
