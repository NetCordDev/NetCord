using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

internal interface IGatewayHandlerMetadata;

internal interface IShardedGatewayHandlerMetadata;

internal interface IHttpInteractionHandlerMetadata;

internal interface IWebhookHandlerMetadata;

internal abstract class HandlerMetadata : IGatewayHandlerMetadata, IShardedGatewayHandlerMetadata, IHttpInteractionHandlerMetadata, IWebhookHandlerMetadata;

internal abstract class ClassHandlerMetadata(Type handlerType) : HandlerMetadata
{
    public static ClassHandlerMetadata Create([DAM(DAMT.PublicConstructors)] Type handlerType, bool isSingleton)
    {
        return isSingleton
            ? SingletonClassHandlerMetadata.Create(handlerType)
            : new NonSingletonClassHandlerMetadata(handlerType, HandlerHelpers.GetHandlerFlags(handlerType));
    }

    public static ClassHandlerMetadata CreateWithFactory(Type handlerType, bool isSingleton, Func<IServiceProvider, object> instanceFactory)
    {
        if (isSingleton)
            return SingletonClassHandlerMetadata.CreateWithFactory(handlerType, instanceFactory);

        return new NonSingletonClassHandlerMetadata(handlerType, instanceFactory, HandlerHelpers.GetHandlerFlags(handlerType) | HandlerFlags.IsNotConcrete);
    }

    public Type HandlerType => handlerType;

    public abstract Func<IServiceProvider, object> InstanceFactory { get; }
}

internal class SingletonClassHandlerMetadata : ClassHandlerMetadata
{
    public sealed override Func<IServiceProvider, object> InstanceFactory { get; }

    protected object? _instance;

    public static SingletonClassHandlerMetadata Create([DAM(DAMT.PublicConstructors)] Type handlerType)
    {
        var isDisposable = HandlerHelpers.IsTypeDisposable(handlerType);
        var isAsyncDisposable = HandlerHelpers.IsTypeAsyncDisposable(handlerType);

        return (isDisposable, isAsyncDisposable) switch
        {
            (false, false) => new SingletonClassHandlerMetadata(handlerType),
            (true, false) => new DisposableSingletonClassHandlerMetadata(handlerType),
            (false, true) => new AsyncDisposableSingletonClassHandlerMetadata(handlerType),
            (true, true) => new DisposableAsyncDisposableSingletonClassHandlerMetadata(handlerType),
        };
    }

    public static SingletonClassHandlerMetadata CreateWithFactory(Type handlerType, Func<IServiceProvider, object> instanceFactory)
    {
        return new FactorySingletonClassHandlerMetadata(handlerType, instanceFactory);
    }

    private SingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= ActivatorUtilities.CreateInstance(services, handlerType);
    }

    private SingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= instanceFactory(services);
    }

    private sealed class DisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata(handlerType), IDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }
    }

    private sealed class AsyncDisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata(handlerType), IAsyncDisposable
    {
        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    private sealed class DisposableAsyncDisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata(handlerType), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }

        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    private sealed class FactorySingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory) : SingletonClassHandlerMetadata(handlerType, instanceFactory), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            if (_instance is IDisposable disposable)
                disposable.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            var instance = _instance;

            if (instance is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            if (instance is IDisposable disposable)
                disposable.Dispose();

            return default;
        }
    }
}

internal sealed class NonSingletonClassHandlerMetadata : ClassHandlerMetadata
{
    public override Func<IServiceProvider, object> InstanceFactory { get; }

    public HandlerFlags Flags { get; }

    public NonSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType, HandlerFlags flags) : base(handlerType)
    {
        var rawFactory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

        InstanceFactory = services => rawFactory(services, null);

        Flags = flags;
    }

    public NonSingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, object> instanceFactory, HandlerFlags flags) : base(handlerType)
    {
        InstanceFactory = instanceFactory;

        Flags = flags;
    }
}

internal sealed class DelegateHandlerMetadata<T>(Delegate handler, T eventId, bool isSingleton) : HandlerMetadata
{
    public bool IsSingleton => isSingleton;

    public Delegate Handler => handler;

    public T EventId => eventId;
}
