using System.Globalization;

namespace NetCord.Rest;

public class ApplicationCommandOption : IJsonModel<JsonModels.JsonApplicationCommandOption>
{
    JsonModels.JsonApplicationCommandOption IJsonModel<JsonModels.JsonApplicationCommandOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandOption _jsonModel;

    public ApplicationCommandOptionType Type => _jsonModel.Type;

    public string Name => _jsonModel.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    public string Description => _jsonModel.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    public bool Required => _jsonModel.Required;

    public IEnumerable<ApplicationCommandOptionChoice>? Choices { get; }

    public IEnumerable<ApplicationCommandOption>? Options { get; }

    public IReadOnlyList<ChannelType>? ChannelTypes => _jsonModel.ChannelTypes;

    public double? MinValue => _jsonModel.MinValue;

    public double? MaxValue => _jsonModel.MaxValue;

    public int? MinLength => _jsonModel.MinLength;

    public int? MaxLength => _jsonModel.MaxLength;

    public bool Autocomplete => _jsonModel.Autocomplete;

    private readonly string _fullName;

    private readonly Snowflake _parentId;

    public ApplicationCommandOption(JsonModels.JsonApplicationCommandOption jsonModel, string parentName, Snowflake parentId)
    {
        _jsonModel = jsonModel;
        _fullName = $"{parentName} {jsonModel.Name}";
        _parentId = parentId;
        if (jsonModel.Choices != null)
            Choices = jsonModel.Choices.Select(c => new ApplicationCommandOptionChoice(c));
        if (jsonModel.Options != null)
            Options = jsonModel.Options.Select(o => new ApplicationCommandOption(o, _fullName, _parentId));
    }

    public override string ToString() => $"</{_fullName}:{_parentId}>";
}