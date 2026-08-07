using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

internal static class HandlerHelpers
{
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
