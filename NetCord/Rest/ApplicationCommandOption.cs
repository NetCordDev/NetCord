namespace NetCord.Rest;

public class ApplicationCommandOption : IJsonModel<JsonModels.JsonApplicationCommandOption>, ISpanFormattable
{
    JsonModels.JsonApplicationCommandOption IJsonModel<JsonModels.JsonApplicationCommandOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandOption _jsonModel;

    /// <summary>
    /// Type of the option.
    /// </summary>
    public ApplicationCommandOptionType Type => _jsonModel.Type;

    /// <summary>
    /// Name of the option (1-32 characters).
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Translations of <see cref="Name"/> (1-32 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations => _jsonModel.NameLocalizations;

    /// <summary>
    /// Description of the option (1-100 characters).
    /// </summary>
    public string Description => _jsonModel.Description;

    /// <summary>
    /// Translations of <see cref="Description"/> (1-100 characters each).
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    /// <summary>
    /// If the parameter is required or optional.
    /// </summary>
    public bool Required => _jsonModel.Required;

    /// <summary>
    /// Choices for the user to pick from (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOptionChoice>? Choices { get; }

    /// <summary>
    /// Parameters for the option (max 25).
    /// </summary>
    public IReadOnlyList<ApplicationCommandOption>? Options { get; }

    /// <summary>
    /// If the option is a channel type, the channels shown will be restricted to these types.
    /// </summary>
    public IReadOnlyList<ChannelType>? ChannelTypes => _jsonModel.ChannelTypes;

    /// <summary>
    /// The minimum value permitted.
    /// </summary>
    public double? MinValue => _jsonModel.MinValue;

    /// <summary>
    /// The maximum value permitted.
    /// </summary>
    public double? MaxValue => _jsonModel.MaxValue;

    /// <summary>
    /// The minimum allowed length (0-6000).
    /// </summary>
    public int? MinLength => _jsonModel.MinLength;

    /// <summary>
    /// The maximum allowed length (0-6000).
    /// </summary>
    public int? MaxLength => _jsonModel.MaxLength;

    /// <summary>
    /// If autocomplete interactions are enabled for the option.
    /// </summary>
    public bool Autocomplete => _jsonModel.Autocomplete;

    private readonly string _fullName;

    private readonly ulong _parentId;

    public ApplicationCommandOption(JsonModels.JsonApplicationCommandOption jsonModel, string parentName, ulong parentId)
    {
        _jsonModel = jsonModel;
        _fullName = $"{parentName} {jsonModel.Name}";
        _parentId = parentId;

        var choices = jsonModel.Choices;
        if (choices is not null)
            Choices = choices.Select(c => new ApplicationCommandOptionChoice(c)).ToArray();

        var options = jsonModel.Options;
        if (options is not null)
            Options = options.Select(o => new ApplicationCommandOption(o, _fullName, _parentId)).ToArray();
    }

    public override string ToString() => $"</{_fullName}:{_parentId}>";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        var requiredLength = 5 + _fullName.Length;
        if (destination.Length < requiredLength || !_parentId.TryFormat(destination[(3 + _fullName.Length)..^1], out int length))
        {
            charsWritten = 0;
            return false;
        }

        "</".CopyTo(destination);
        _fullName.CopyTo(destination[2..]);
        destination[2 + _fullName.Length] = ':';
        destination[3 + _fullName.Length + length] = '>';

        charsWritten = 4 + _fullName.Length + length;
        return true;
    }
}
