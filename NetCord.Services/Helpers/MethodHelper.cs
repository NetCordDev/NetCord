using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class MethodHelper
{
    public static void EnsureNoParameters(MethodInfo method) => EnsureNoParameters(method.GetParameters(), method);

    public static void EnsureNoParameters(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method)
    {
        if (parameters.Length != 0)
            throw new InvalidDefinitionException("The command cannot have parameters.", method);
    }
}
