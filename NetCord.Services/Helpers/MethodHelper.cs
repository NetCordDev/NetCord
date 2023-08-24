using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class MethodHelper
{
    public static void EnsureMethodReturnTypeValid(MethodInfo method)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"The method must return '{typeof(Task)}'.", method);
    }

    public static void EnsureMethodParameterless(MethodInfo method)
    {
        if (method.GetParameters().Length != 0)
            throw new InvalidDefinitionException("The method must be parameterless.", method);
    }
}
