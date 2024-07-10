namespace NetCord.Rest;

public class ApplicationCommandGuildPermissions(JsonModels.JsonApplicationCommandGuildPermissions jsonModel) : IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>
{
    JsonModels.JsonApplicationCommandGuildPermissions IJsonModel<JsonModels.JsonApplicationCommandGuildPermissions>.JsonModel => jsonModel;

    /// <summary>
    /// ID of the command.
    /// </summary>
    public ulong CommandId => jsonModel.CommandId;

    /// <summary>
    /// ID of the application the command belongs to.
    /// </summary>
    public ulong ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// ID of the guild.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// Permissions for the command in the guild (max 100).
    /// </summary>
    public IReadOnlyDictionary<ulong, ApplicationCommandPermission> Permissions { get; } = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandPermission(p));
}
