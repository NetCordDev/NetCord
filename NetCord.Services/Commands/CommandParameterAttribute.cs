using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    public bool Remainder { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }
}
