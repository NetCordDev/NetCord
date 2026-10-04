namespace NetCord.Rest;

public class ApplicationCommandOption(JsonModels.JsonApplicationCommandOption jsonModel, string parentName, ulong parentId) : ISpanFormattable
{
    /// <summary>
    /// Type of the option.
    /// </summary>
    public ApplicationCommandOptionType Type { get; } = jsonModel.Type;

    /// <summary>
    /// Name of the option (1-32 characters).
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; } = jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the option (1-100 characters).
    /// </summary>
    public string Description { get; } = jsonModel.Description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; } = jsonModel.DescriptionLocalizations;

    /// <summary>
    /// If the parameter is required or optional.
    /// </summary>
    public bool Required { get; } = jsonModel.Required;

    /// <summary>
    /// Choices for the user to pick from (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOptionChoice>? Choices { get; } = jsonModel.Choices?.Select(c => new ApplicationCommandOptionChoice(c)).ToArray();

    /// <summary>
    /// Parameters for the option (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption>? Options { get; } = jsonModel.Options?.Select(o => new ApplicationCommandOption(o, $"{parentName} {jsonModel.Name}", parentId)).ToArray();

    /// <summary>
    /// If the option is a channel type, the channels shown will be restricted to these types.
    /// </summary>
    public IReadOnlyList<ChannelType>? ChannelTypes { get; } = jsonModel.ChannelTypes;

    /// <summary>
    /// The minimum value permitted.
    /// </summary>
    public double? MinValue { get; } = jsonModel.MinValue;

    /// <summary>
    /// The maximum value permitted.
    /// </summary>
    public double? MaxValue { get; } = jsonModel.MaxValue;

    /// <summary>
    /// The minimum allowed length (0-6000).
    /// </summary>
    public int? MinLength { get; } = jsonModel.MinLength;

    /// <summary>
    /// The maximum allowed length (0-6000).
    /// </summary>
    public int? MaxLength { get; } = jsonModel.MaxLength;

    /// <summary>
    /// If autocomplete interactions are enabled for the option.
    /// </summary>
    public bool Autocomplete { get; } = jsonModel.Autocomplete;

    /// <summary>
    /// File types to filter for; can be <c>image</c>, <c>video</c>, <c>audio</c>, or any dot-prefixed extension such as <c>.pdf</c> (max 10).
    /// </summary>
    public IReadOnlyList<string>? FileTypes { get; } = jsonModel.FileTypes;

    public override string ToString() => $"</{parentName} {Name}:{parentId}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var requiredLength = 6 + parentName.Length + Name.Length;
        if (destination.Length < requiredLength || !parentId.TryFormat(destination[(4 + parentName.Length + Name.Length)..^1], out int length))
        {
            charsWritten = 0;
            return false;
        }

        "</".CopyTo(destination);
        parentName.CopyTo(destination[2..]);
        destination[2 + parentName.Length] = ' ';
        Name.CopyTo(destination[(3 + parentName.Length)..]);
        destination[3 + parentName.Length + Name.Length] = ':';
        destination[4 + parentName.Length + Name.Length + length] = '>';

        charsWritten = 5 + parentName.Length + Name.Length + length;
        return true;
    }
}
