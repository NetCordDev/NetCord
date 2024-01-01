using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? NameTranslationsProviderType { get; init; }

    public string? Description { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? DescriptionTranslationsProviderType { get; init; }

    public double MaxValue
    {
        get => _maxValue.GetValueOrDefault();
        init
        {
            _maxValue = value;
        }
    }

    internal readonly double? _maxValue;

    public double MinValue
    {
        get => _minValue.GetValueOrDefault();
        init
        {
            _minValue = value;
        }
    }

    internal readonly double? _minValue;

    public int MaxLength
    {
        get => _maxLength.GetValueOrDefault();
        init
        {
            _maxLength = value;
        }
    }

    internal readonly int? _maxLength;

    public int MinLength
    {
        get => _minLength.GetValueOrDefault();
        init
        {
            _minLength = value;
        }
    }

    internal readonly int? _minLength;

    public ChannelType[]? AllowedChannelTypes { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? ChoicesProviderType { get; init; }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? AutocompleteProviderType { get; init; }
}
