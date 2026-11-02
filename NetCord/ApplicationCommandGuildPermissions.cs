using NetCord.JsonModels;

namespace NetCord;

public class ApplicationCommandGuildPermissions(JsonApplicationCommandGuildPermissions jsonModel)
{
    /// <summary>
    /// ID of the command.
    /// </summary>
    public ulong CommandId { get; } = jsonModel.CommandId;

    /// <summary>
    /// ID of the application the command belongs to.
    /// </summary>
    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// ID of the guild.
    /// </summary>
    public ulong GuildId { get; } = jsonModel.GuildId;

    /// <summary>
    /// Permissions for the command in the guild (max 100).
    /// </summary>
    public IReadOnlyDictionary<ulong, ApplicationCommandGuildPermission> Permissions { get; } = jsonModel.Permissions.ToDictionary(p => p.Id, p => new ApplicationCommandGuildPermission(p));
}
