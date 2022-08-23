using System.Globalization;

namespace NetCord.Rest;

public class ApplicationCommand : ClientEntity, IJsonModel<JsonModels.JsonApplicationCommand>
{
    JsonModels.JsonApplicationCommand IJsonModel<JsonModels.JsonApplicationCommand>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonApplicationCommand _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public ApplicationCommandType Type => _jsonModel.Type;

    public Snowflake ApplicationId => _jsonModel.ApplicationId;

    public string Name => _jsonModel.Name;

    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations => _jsonModel.NameLocalizations;

    public string Description => _jsonModel.Description;

    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations => _jsonModel.DescriptionLocalizations;

    public Permission? DefaultGuildUserPermissions => _jsonModel.DefaultGuildUserPermissions;

    public bool DMPermission => _jsonModel.DMPermission.GetValueOrDefault();

    public IEnumerable<ApplicationCommandOption> Options { get; }

    public bool DefaultPermission => _jsonModel.DefaultPermission;

    public Snowflake Version => _jsonModel.Version;

    public ApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Options = jsonModel.Options.SelectOrEmpty(o => new ApplicationCommandOption(o, jsonModel.Name, jsonModel.Id));
    }

    public override string ToString() => $"</{Name}:{Id}>";

    #region Interactions.ApplicationCommands
    public virtual Task<ApplicationCommand> ModifyAsync(Action<ApplicationCommandOptions> action, RequestProperties? properties = null) => _client.ModifyGlobalApplicationCommandAsync(ApplicationId, Id, action, properties);
    public virtual Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGlobalApplicationCommandAsync(ApplicationId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> GetGuildPermissionsAsync(Snowflake guildId, RequestProperties? properties = null) => _client.GetApplicationCommandGuildPermissionsAsync(ApplicationId, guildId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> OverwriteGuildPermissionsAsync(Snowflake guildId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null) => _client.OverwriteApplicationCommandGuildPermissionsAsync(ApplicationId, guildId, Id, newPermissions, properties);
    #endregion
}