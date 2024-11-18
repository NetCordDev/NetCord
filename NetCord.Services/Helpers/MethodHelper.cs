using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class MethodHelper
{
    public static bool EnsureSingleParameterOfTypeOrNone(MethodInfo method, Type parameterType)
    {
        return EnsureSingleParameterOfTypeOrNone(method.GetParameters(), method, parameterType);
    }

    public static bool EnsureSingleParameterOfTypeOrNone(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, Type parameterType)
    {
        switch (parameters.Length)
        {
            case 0:
                return false;
            case 1:
                if (parameters[0].ParameterType == parameterType)
                    return true;
                goto default;
            default:
                throw new InvalidDefinitionException($"The command must have no parameters or a single parameter of type '{parameterType}'.", method);
        }
    }
}
