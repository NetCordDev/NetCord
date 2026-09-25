using NetCord.JsonModels;

namespace NetCord;

public class ApplicationCommandGuildPermissions(JsonApplicationCommandGuildPermissions jsonModel) : IJsonModel<JsonApplicationCommandGuildPermissions>
{
    JsonApplicationCommandGuildPermissions IJsonModel<JsonApplicationCommandGuildPermissions>.JsonModel => jsonModel;

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
    public IReadOnlyDictionary<ulong, ApplicationCommandGuildPermission> Permissions { get; } = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandGuildPermission(p));
}
