using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

internal interface IGatewayHandlerMetadata;

internal interface IShardedGatewayHandlerMetadata;

internal interface IHttpInteractionHandlerMetadata;

internal interface IWebhookHandlerMetadata;

internal abstract class HandlerMetadata : IGatewayHandlerMetadata, IShardedGatewayHandlerMetadata, IHttpInteractionHandlerMetadata, IWebhookHandlerMetadata;

internal abstract class ClassHandlerMetadata<T> : HandlerMetadata where T : class
{
    public ClassHandlerMetadata(Type handlerType)
    {
        Debug.Assert(typeof(T).IsAssignableFrom(handlerType));

        HandlerType = handlerType;
    }

    public static ClassHandlerMetadata<T> Create([DAM(DAMT.PublicConstructors)] Type handlerType, bool isSingleton)
    {
        return isSingleton
            ? SingletonClassHandlerMetadata<T>.Create(handlerType)
            : new NonSingletonClassHandlerMetadata<T>(handlerType, HandlerHelpers.GetHandlerFlags(handlerType));
    }

    public static ClassHandlerMetadata<T> CreateWithFactory(Type handlerType, bool isSingleton, Func<IServiceProvider, T> instanceFactory)
    {
        if (isSingleton)
            return SingletonClassHandlerMetadata<T>.CreateWithFactory(handlerType, instanceFactory);

        return new NonSingletonClassHandlerMetadata<T>(handlerType, instanceFactory, HandlerHelpers.GetHandlerFlags(handlerType) | HandlerFlags.IsNotConcrete);
    }

    public Type HandlerType { get; }

    public required Func<IServiceProvider, T> InstanceFactory { get; init; }
}

internal class SingletonClassHandlerMetadata<T> : ClassHandlerMetadata<T> where T : class
{
    protected T? _instance;

    public static SingletonClassHandlerMetadata<T> Create([DAM(DAMT.PublicConstructors)] Type handlerType)
    {
        var isDisposable = HandlerHelpers.IsTypeDisposable(handlerType);
        var isAsyncDisposable = HandlerHelpers.IsTypeAsyncDisposable(handlerType);

        return (isDisposable, isAsyncDisposable) switch
        {
            (false, false) => new SingletonClassHandlerMetadata<T>(handlerType),
            (true, false) => new DisposableSingletonClassHandlerMetadata(handlerType),
            (false, true) => new AsyncDisposableSingletonClassHandlerMetadata(handlerType),
            (true, true) => new DisposableAsyncDisposableSingletonClassHandlerMetadata(handlerType),
        };
    }

    public static SingletonClassHandlerMetadata<T> CreateWithFactory(Type handlerType, Func<IServiceProvider, T> instanceFactory)
    {
        return new FactorySingletonClassHandlerMetadata(handlerType, instanceFactory);
    }

    [SetsRequiredMembers]
    private SingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= Unsafe.As<T>(ActivatorUtilities.CreateInstance(services, handlerType));
    }

    [SetsRequiredMembers]
    private SingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, T> instanceFactory) : base(handlerType)
    {
        InstanceFactory = services => _instance ??= instanceFactory(services);
    }

    [method: SetsRequiredMembers]
    private sealed class DisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata<T>(handlerType), IDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }
    }

    [method: SetsRequiredMembers]
    private sealed class AsyncDisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata<T>(handlerType), IAsyncDisposable
    {
        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    [method: SetsRequiredMembers]
    private sealed class DisposableAsyncDisposableSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType) : SingletonClassHandlerMetadata<T>(handlerType), IDisposable, IAsyncDisposable
    {
        public void Dispose()
        {
            HandlerHelpers.DisposeInstance(_instance);
        }

        public ValueTask DisposeAsync() => HandlerHelpers.DisposeInstanceAsync(_instance);
    }

    [method: SetsRequiredMembers]
    private sealed class FactorySingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, T> instanceFactory) : SingletonClassHandlerMetadata<T>(handlerType, instanceFactory), IDisposable, IAsyncDisposable
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

internal sealed class NonSingletonClassHandlerMetadata<T> : ClassHandlerMetadata<T> where T : class
{
    public HandlerFlags Flags { get; }

    [SetsRequiredMembers]
    public NonSingletonClassHandlerMetadata([DAM(DAMT.PublicConstructors)] Type handlerType, HandlerFlags flags) : base(handlerType)
    {
        var rawFactory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

        InstanceFactory = services => Unsafe.As<T>(rawFactory(services, null));

        Flags = flags;
    }

    [SetsRequiredMembers]
    public NonSingletonClassHandlerMetadata(Type handlerType, Func<IServiceProvider, T> instanceFactory, HandlerFlags flags) : base(handlerType)
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
