using System.Globalization;

namespace NetCord;

public class ApplicationCommand : Entity
{
    private readonly JsonModels.JsonApplicationCommand _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;

    public ApplicationCommandType Type => _jsonEntity.Type;

    public Snowflake ApplicationId => _jsonEntity.ApplicationId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public string Name => _jsonEntity.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonEntity.NameLocalizations;

    public string Description => _jsonEntity.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonEntity.DescriptionLocalizations;

    public IEnumerable<ApplicationCommandOption> Options { get; }

    public bool DefaultPermission => _jsonEntity.DefaultPermission;

    public Snowflake Version => _jsonEntity.Version;

    internal ApplicationCommand(JsonModels.JsonApplicationCommand jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Options = jsonEntity.Options.SelectOrEmpty(o => new ApplicationCommandOption(o));
    }
}