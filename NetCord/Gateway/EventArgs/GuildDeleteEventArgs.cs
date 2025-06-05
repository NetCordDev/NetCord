using NetCord.JsonModels;

namespace NetCord.Gateway;

public class GuildDeleteEventArgs(JsonGuild jsonModel) : IJsonModel<JsonGuild>
{
    JsonGuild IJsonModel<JsonGuild>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild.
    /// </summary>
    public ulong GuildId => jsonModel.Id;

    /// <summary>
    /// Whether the guild is unavailable. If <see langword="false"/>, the bot was removed from the guild.
    /// </summary>
    public bool IsUnavailable => jsonModel.IsUnavailable;
}
