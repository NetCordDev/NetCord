using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents an application command option.
/// </summary>
public class ApplicationCommandOption(JsonApplicationCommandOption jsonModel, string parentName, ulong parentId) : IJsonModel<JsonApplicationCommandOption>, ISpanFormattable
{
    JsonApplicationCommandOption IJsonModel<JsonApplicationCommandOption>.JsonModel => jsonModel;

    /// <summary>
    /// Type of the option.
    /// </summary>
    public ApplicationCommandOptionType Type => jsonModel.Type;

    /// <summary>
    /// Name of the option (1-32 characters).
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// Localizations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations => jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the option (1-100 characters).
    /// </summary>
    public string Description => jsonModel.Description;

    /// <summary>
    /// Localizations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations => jsonModel.DescriptionLocalizations;

    /// <summary>
    /// If the parameter is required or optional.
    /// </summary>
    public bool Required => jsonModel.Required;

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
    public IReadOnlyList<ChannelType>? ChannelTypes => jsonModel.ChannelTypes;

    /// <summary>
    /// The minimum value permitted.
    /// </summary>
    public double? MinValue => jsonModel.MinValue;

    /// <summary>
    /// The maximum value permitted.
    /// </summary>
    public double? MaxValue => jsonModel.MaxValue;

    /// <summary>
    /// The minimum allowed length (0-6000).
    /// </summary>
    public int? MinLength => jsonModel.MinLength;

    /// <summary>
    /// The maximum allowed length (0-6000).
    /// </summary>
    public int? MaxLength => jsonModel.MaxLength;

    /// <summary>
    /// If autocomplete interactions are enabled for the option.
    /// </summary>
    public bool Autocomplete => jsonModel.Autocomplete;

    /// <summary>
    /// File types to filter for; can be <c>image</c>, <c>video</c>, <c>audio</c>, or any dot-prefixed extension such as <c>.pdf</c> (max 10).
    /// </summary>
    public IReadOnlyList<string>? FileTypes => jsonModel.FileTypes;

    private readonly string _fullName = $"{parentName} {jsonModel.Name}";

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return string.Create(formatProvider, $"</{_fullName}:{parentId}>");
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return destination.TryWrite(provider, $"</{_fullName}:{parentId}>", out charsWritten);
    }
}
