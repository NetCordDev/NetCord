using System.Reflection;

namespace NetCord.Services;

public class InvalidDefinitionException : Exception
{
    public MethodInfo Method { get; }

    internal InvalidDefinitionException(string? message, MethodInfo method) : base(message + $" | {method.DeclaringType!.FullName}.{method.Name}")
    {
        Method = method;
    }
}
