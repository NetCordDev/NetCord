using System.Globalization;

namespace NetCord;

public class ApplicationCommandOption
{
    private readonly JsonModels.JsonApplicationCommandOption _jsonEntity;

    public ApplicationCommandOptionType Type => _jsonEntity.Type;

    public string Name => _jsonEntity.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonEntity.NameLocalizations;

    public string Description => _jsonEntity.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonEntity.DescriptionLocalizations;

    public bool Required => _jsonEntity.Required;

    public IEnumerable<ApplicationCommandOptionChoice>? Choices { get; }

    public IEnumerable<ApplicationCommandOption>? Options { get; }

    public IEnumerable<ChannelType>? ChannelTypes => _jsonEntity.ChannelTypes;

    public double? MinValue => _jsonEntity.MinValue;

    public double? MaxValue => _jsonEntity.MaxValue;

    public bool Autocomplete => _jsonEntity.Autocomplete;

    internal ApplicationCommandOption(JsonModels.JsonApplicationCommandOption jsonEntity)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Choices != null)
            Choices = jsonEntity.Choices.Select(c => new ApplicationCommandOptionChoice(c));
        if (jsonEntity.Options != null)
            Options = jsonEntity.Options.Select(o => new ApplicationCommandOption(o));
    }
}