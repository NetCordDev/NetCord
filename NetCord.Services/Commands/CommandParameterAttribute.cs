using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.Commands;

/// <summary>
/// Specifies metadata for a parameter of a command.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Indicates whether the parameter is parsed as a remainder, meaning that it captures the rest of the input after the command and preceding parameters.
    /// </summary>
    public bool Remainder { get; init; }

    /// <summary>
    /// Type reader for the parameter, used to convert the input value to the specified type.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }
}
