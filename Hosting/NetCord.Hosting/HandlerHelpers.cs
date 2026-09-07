using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

    public static void EnsureHandlerTypeIsValid(Type handlerType, Type baseType)
    {
        if (!baseType.IsAssignableFrom(handlerType))
            ThrowInvalidHandler(handlerType, baseType);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidHandler(Type handlerType, Type baseType)
        {
            throw new ArgumentException($"The type '{handlerType.FullName}' is not a valid '{baseType.FullName}'.", nameof(handlerType));
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
