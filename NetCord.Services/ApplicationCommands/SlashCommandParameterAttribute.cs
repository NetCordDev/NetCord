using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? NameTranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }

    public string? Description { get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? DescriptionTranslationsProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }

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

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? TypeReaderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type? ChoicesProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] get; init; }

    [field: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public Type? AutocompleteProviderType { [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] get; init; }
}
