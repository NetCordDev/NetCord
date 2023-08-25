using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    public bool Remainder { get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }
}
