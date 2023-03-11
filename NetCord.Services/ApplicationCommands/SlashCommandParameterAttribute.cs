namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class SlashCommandParameterAttribute : Attribute
{
    public string? Name { get; init; }

    public Type? NameTranslationsProviderType { get; init; }

    public string? Description { get; init; }

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

    public Type? TypeReaderType { get; init; }

    public Type? ChoicesProviderType { get; init; }

    public Type? AutocompleteProviderType { get; init; }
}
