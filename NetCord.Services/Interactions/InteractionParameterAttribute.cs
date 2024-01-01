using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Parameter)]
public class InteractionParameterAttribute : Attribute
{
    public string? Name { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }
}
