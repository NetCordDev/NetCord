using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class ServiceHelpers
{
    [RequiresUnreferencedCode("Types might be removed")]
    public static IEnumerable<Type> GetModules(Type baseType, Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsAbstract || type.IsNested)
                continue;

            if (baseType.IsAssignableFrom(type))
                yield return type;
        }
    }
}
