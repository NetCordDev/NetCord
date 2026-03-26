using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Hosting;

internal static class HandlerHelpers
{
    [RequiresUnreferencedCode("Types might be removed")]
    public static IEnumerable<Type> GetHandlers(Type baseType, Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(type => !type.IsAbstract && !type.IsNested && baseType.IsAssignableFrom(type));
    }
}
