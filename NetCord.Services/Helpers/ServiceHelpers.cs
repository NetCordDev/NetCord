using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class ServiceHelpers
{
    [RequiresUnreferencedCode("Types might be removed")]
    public static IEnumerable<Type> GetTopLevelModules(Type baseType, Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(type => IsTopLevelModule(baseType, type));
    }

    public static bool IsTopLevelModule(Type baseType, Type type)
    {
        return !type.IsNested && IsModule(baseType, type);
    }

    public static bool IsModule(Type baseType, Type type)
    {
        return !type.IsAbstract && baseType.IsAssignableFrom(type);
    }
}
