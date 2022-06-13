using System.Globalization;

namespace NetCord;

public class ApplicationCommand : Entity, IJsonModel<JsonModels.JsonApplicationCommand>
{
    JsonModels.JsonApplicationCommand IJsonModel<JsonModels.JsonApplicationCommand>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommand _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public ApplicationCommandType Type => _jsonModel.Type;

    public Snowflake ApplicationId => _jsonModel.ApplicationId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public string Name => _jsonModel.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    public string Description => _jsonModel.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    public Permission? DefaultGuildUserPermissions { get; }

    public bool? DMPermission => _jsonModel.DMPermission;

    public IEnumerable<ApplicationCommandOption> Options { get; }

    public bool DefaultPermission => _jsonModel.DefaultPermission;

    public Snowflake Version => _jsonModel.Version;

    public ApplicationCommand(JsonModels.JsonApplicationCommand jsonModel)
    {
        _jsonModel = jsonModel;
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandOption(o));
        if (jsonModel.DefaultGuildUserPermissions != null)
            DefaultGuildUserPermissions = (Permission)ulong.Parse(jsonModel.DefaultGuildUserPermissions);
    }
}