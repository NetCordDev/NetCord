using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ComponentInteractions;

[AttributeUsage(AttributeTargets.Parameter)]
public class ComponentInteractionParameterAttribute : Attribute
{
    public string? Name { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }
}
