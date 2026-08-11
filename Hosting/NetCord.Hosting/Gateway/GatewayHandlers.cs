using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Gateway;

public interface IGatewayHandler;

public interface IShardedGatewayHandler;

internal abstract class GatewayHandlerMetadata
{
}

internal abstract class ClassGatewayHandlerMetadata(Type handlerType) : GatewayHandlerMetadata
{
    public static ClassGatewayHandlerMetadata Create([DAM(DAMT.PublicConstructors)] Type handlerType, bool isSingleton)
    {
        return isSingleton
            ? SingletonClassGatewayHandlerMetadata.Create(handlerType)
            : new NonSingletonClassGatewayHandlerMetadata(handlerType, HandlerHelpers.GetHandlerFlags(handlerType));
    }

    public static ClassGatewayHandlerMetadata CreateWithCustomFactory(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory)
    {
        if (isSingleton)
            return SingletonClassGatewayHandlerMetadata.CreateWithFactory(handlerType, instanceFactory);

        return new NonSingletonClassGatewayHandlerMetadata(handlerType, instanceFactory, HandlerHelpers.GetHandlerFlags(handlerType) | HandlerFlags.IsNotConcrete);
    }

    public Type HandlerType => handlerType;
}

internal class SingletonClassGatewayHandlerMetadata : ClassGatewayHandlerMetadata
{
    public Func<IServiceProvider, object> InstanceFactory { get; }

    protected object? _instance;

    public static SingletonClassGatewayHandlerMetadata Create([DAM(DAMT.PublicConstructors)] Type handlerType)
    {
        var isDisposable = HandlerHelpers.IsTypeDisposable(handlerType);
        var isAsyncDisposable = HandlerHelpers.IsTypeAsyncDisposable(handlerType);

        return (isDisposable, isAsyncDisposable) switch
        {
            (false, false) => new SingletonClassGatewayHandlerMetadata(handlerType),
            (true, false) => new DisposableSingletonClassGatewayHandlerMetadata(handlerType),
            (false, true) => new AsyncDisposableSingletonClassGatewayHandlerMetadata(handlerType),
            (true, true) => new DisposableAsyncDisposableSingletonClassGatewayHandlerMetadata(handlerType),
        };
    }

    public static SingletonClassGatewayHandlerMetadata CreateWithFactory(Type handlerType, Func<IServiceProvider, object> instanceFactory)
    {
        return new FactorySingletonClassGatewayHandlerMetadata(handlerType, instanceFactory);
    }

    private SingletonClassGatewayHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= ActivatorUtilities.CreateInstance(services, handlerType);
    }

    private SingletonClassGatewayHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= instanceFactory(services);
    }

    private sealed class DisposableSingletonClassGatewayHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassGatewayHandlerMetadata(handlerType), IDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }
    }

    private sealed class AsyncDisposableSingletonClassGatewayHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassGatewayHandlerMetadata(handlerType), IAsyncDisposable
    {
        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    private sealed class DisposableAsyncDisposableSingletonClassGatewayHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassGatewayHandlerMetadata(handlerType), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }

        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    private sealed class FactorySingletonClassGatewayHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory) : SingletonClassGatewayHandlerMetadata(handlerType, instanceFactory), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            if (_instance is IDisposable disposable)
                disposable.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            if (_instance is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            if (_instance is IDisposable disposable)
                disposable.Dispose();

            return default;
        }
    }
}

internal sealed class NonSingletonClassGatewayHandlerMetadata : ClassGatewayHandlerMetadata
{
    public Func<IServiceProvider, object> InstanceFactory { get; }

    public HandlerFlags Flags { get; }

    public NonSingletonClassGatewayHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType, HandlerFlags flags) : base(handlerType)
    {
        var rawFactory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

        InstanceFactory = services => rawFactory(services, null);

        Flags = flags;
    }

    public NonSingletonClassGatewayHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory, HandlerFlags flags) : base(handlerType)
    {
        InstanceFactory = instanceFactory;

        Flags = flags;
    }
}

internal sealed class DelegateGatewayHandlerMetadata(Delegate handler, GatewayEventId eventId, bool isSingleton) : GatewayHandlerMetadata
{
    public bool IsSingleton => isSingleton;

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
