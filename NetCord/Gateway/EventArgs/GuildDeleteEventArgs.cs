using NetCord.Gateway.JsonModels;

namespace NetCord.Gateway;

public class GuildDeleteEventArgs(JsonUnavailableGuild jsonModel)
{
    /// <summary>
    /// The ID of the guild.
    /// </summary>
    public ulong GuildId { get; } = jsonModel.Id;

    /// <summary>
    /// Whether the guild is unavailable. If <see langword="false"/>, the bot was removed from the guild.
    /// </summary>
    public bool IsUnavailable { get; } = jsonModel.IsUnavailable.GetValueOrDefault();
}
