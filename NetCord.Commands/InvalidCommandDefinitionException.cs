using System.Reflection;

namespace NetCord.Commands;

public class InvalidCommandDefinitionException : Exception
{
    internal InvalidCommandDefinitionException(string? message, MethodInfo method) : base(message + $" | {method.DeclaringType!.FullName}.{method.Name}")
    {
    }
}