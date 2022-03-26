using System.Globalization;

namespace NetCord;

public class ApplicationCommand : Entity
{
    private readonly JsonModels.JsonApplicationCommand _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;

    public ApplicationCommandType Type => _jsonEntity.Type;

    public DiscordId ApplicationId => _jsonEntity.ApplicationId;

    public DiscordId? GuildId => _jsonEntity.GuildId;

    public string Name => _jsonEntity.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonEntity.NameLocalizations;

    public string Description => _jsonEntity.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonEntity.DescriptionLocalizations;

    public IEnumerable<ApplicationCommandOption> Options { get; }

    public bool DefaultPermission => _jsonEntity.DefaultPermission;

    public DiscordId Version => _jsonEntity.Version;

    internal ApplicationCommand(JsonModels.JsonApplicationCommand jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Options = jsonEntity.Options.SelectOrEmpty(o => new ApplicationCommandOption(o));
    }
}