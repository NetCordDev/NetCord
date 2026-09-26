using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Specifies metadata for a parameter of a component interaction.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class ComponentInteractionParameterAttribute : Attribute
{
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Type reader for the parameter, used to convert the input value to the specified type.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }
}
