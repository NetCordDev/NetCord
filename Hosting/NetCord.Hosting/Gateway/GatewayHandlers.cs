namespace NetCord.Hosting.Gateway;

public interface IGatewayHandler;

public interface IShardedGatewayHandler;

internal abstract class GatewayHandlerMetadata(bool isSingleton)
{
    public bool IsSingleton => isSingleton;
}

internal sealed class ClassGatewayHandlerMetadata(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory) : GatewayHandlerMetadata(isSingleton)
{
    public Type HandlerType => handlerType;

    public Func<IServiceProvider, object> InstanceFactory => instanceFactory;
}

internal sealed class DelegateGatewayHandlerMetadata(Delegate handler, GatewayEventId eventId, bool isSingleton) : GatewayHandlerMetadata(isSingleton)
{
    public Delegate Handler => handler;

    public GatewayEventId EventId => eventId;
}

internal abstract class ShardedGatewayHandlerMetadata(bool isSingleton)
{
    public bool IsSingleton => isSingleton;
}

internal sealed class ClassShardedGatewayHandlerMetadata(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory) : ShardedGatewayHandlerMetadata(isSingleton)
{
    public Type HandlerType => handlerType;

    public Func<IServiceProvider, object> InstanceFactory => instanceFactory;
}

internal sealed class DelegateShardedGatewayHandlerMetadata(Delegate handler, GatewayEventId eventId, bool isSingleton) : ShardedGatewayHandlerMetadata(isSingleton)
{
    public Delegate Handler => handler;

    public GatewayEventId EventId => eventId;
}
