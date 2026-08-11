using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

internal static class HandlerHelpers
{
    public static bool IsTypeDisposable(Type type) => typeof(IDisposable).IsAssignableFrom(type);

    public static bool IsTypeAsyncDisposable(Type type) => typeof(IAsyncDisposable).IsAssignableFrom(type);

    public static HandlerFlags GetHandlerFlags(Type type)
    {
        return GetHandlerFlags(IsTypeDisposable(type), IsTypeAsyncDisposable(type));
    }

    public static HandlerFlags GetHandlerFlags(bool isDisposable, bool isAsyncDisposable)
    {
        return (isDisposable ? HandlerFlags.IsDisposable : 0) | (isAsyncDisposable ? HandlerFlags.IsAsyncDisposable : 0);
    }

    public static void DisposeInstance(object? instance)
    {
        if (instance is not null)
            ((IDisposable)instance).Dispose();
    }

    public static ValueTask DisposeInstanceAsync(object? instance)
    {
        if (instance is not null)
            return ((IAsyncDisposable)instance).DisposeAsync();

        return default;
    }

    public static Func<IServiceProvider, object> CreateInstanceFactory([DAM(DAMT.PublicConstructors)] Type handlerType, bool isSingleton)
    {
        if (isSingleton)
            return services => ActivatorUtilities.CreateInstance(services, handlerType);
        else
        {
            var rawFactory = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);

            return services => rawFactory(services, null);
        }
    }

    [RequiresUnreferencedCode("Types might be removed")]
    public static IEnumerable<Type> GetHandlers(Type baseType, Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(type => !type.IsAbstract && !type.IsNested && baseType.IsAssignableFrom(type));
    }
}

[Flags]
internal enum HandlerFlags : byte
{
    IsDisposable = 1 << 0,
    IsAsyncDisposable = 1 << 1,
    IsNotConcrete = 1 << 2,
}
