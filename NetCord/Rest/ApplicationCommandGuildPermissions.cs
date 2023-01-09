namespace NetCord.Rest;

public class ApplicationCommandGuildPermissions : IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>
{
    JsonModels.JsonApplicationCommandGuildPermissions IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationCommandGuildPermissions _jsonModel;

    /// <summary>
    /// Id of the command.
    /// </summary>
    public ulong CommandId => _jsonModel.CommandId;

    /// <summary>
    /// Id of the application the command belongs to.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Id of the guild.
    /// </summary>
    public ulong GuildId => _jsonModel.GuildId;

    /// <summary>
    /// Permissions for the command in the guild (max 100).
    /// </summary>
    public IReadOnlyDictionary<ulong, ApplicationCommandPermission> Permissions { get; }

    public ApplicationCommandGuildPermissions(JsonModels.JsonApplicationCommandGuildPermissions jsonModel)
    {
        _jsonModel = jsonModel;
        Permissions = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
    }
}
