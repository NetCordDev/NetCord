using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace NetCord.Services;

[Serializable]
public class InvalidDefinitionException : Exception
{
    [AllowNull]
    [field: NonSerialized]
    public MethodInfo Method { get; }

    public InvalidDefinitionException(string? message, MethodInfo method) : base(message + $" | {method.DeclaringType}.{method.Name}")
    {
        Method = method;
    }

    protected InvalidDefinitionException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}
