using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Specifies metadata for a parameter of a slash command.
/// Use this attribute to configure how a parameter is presented and validated.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    /// <summary>
    /// Name of the parameter (1-32 characters).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Description of the parameter (1-100 characters).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Maximum value permitted for the parameter.
    /// </summary>
    public double MaxValue
    {
        get => _maxValue.GetValueOrDefault();
        init
        {
            _maxValue = value;
        }
    }

    internal readonly double? _maxValue;

    /// <summary>
    /// Minimum value permitted for the parameter.
    /// </summary>
    public double MinValue
    {
        get => _minValue.GetValueOrDefault();
        init
        {
            _minValue = value;
        }
    }

    internal readonly double? _minValue;

    /// <summary>
    /// Maximum length of the parameter value (1-6000).
    /// </summary>
    public int MaxLength
    {
        get => _maxLength.GetValueOrDefault();
        init
        {
            _maxLength = value;
        }
    }

    internal readonly int? _maxLength;

    /// <summary>
    /// Minimum length of the parameter value (0-6000).
    /// </summary>
    public int MinLength
    {
        get => _minLength.GetValueOrDefault();
        init
        {
            _minLength = value;
        }
    }

    internal readonly int? _minLength;

    /// <summary>
    /// If the option is a channel type, the channels shown will be restricted to these types.
    /// </summary>
    public ChannelType[]? AllowedChannelTypes { get; init; }

    /// <summary>
    /// Type reader for the parameter, used to convert the input value to the specified type.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }

    /// <summary>
    /// Provider for choices for the parameter, allowing users to select from predefined options.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? ChoicesProviderType { get; init; }

    /// <summary>
    /// Provider for autocomplete functionality, allowing dynamic suggestions based on user input.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? AutocompleteProviderType { get; init; }
}
