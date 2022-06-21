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

    public IEnumerable<ChannelType>? ChannelTypes => _jsonModel.ChannelTypes;

    public double? MinValue => _jsonModel.MinValue;

    public double? MaxValue => _jsonModel.MaxValue;

    public bool Autocomplete => _jsonModel.Autocomplete;

    public ApplicationCommandOption(JsonModels.JsonApplicationCommandOption jsonModel)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Choices != null)
            Choices = jsonModel.Choices.Select(c => new ApplicationCommandOptionChoice(c));
        if (jsonModel.Options != null)
            Options = jsonModel.Options.Select(o => new ApplicationCommandOption(o));
    }
}