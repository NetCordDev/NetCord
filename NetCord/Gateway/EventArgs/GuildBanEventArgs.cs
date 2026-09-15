using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a guild ban add or remove event.
/// </summary>
public class GuildBanEventArgs(JsonModels.EventArgs.JsonGuildBanEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonGuildBanEventArgs>
{
    JsonModels.EventArgs.JsonGuildBanEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildBanEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild where the ban event occurred.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The user affected by the guild ban.
    /// </summary>
    public User User { get; } = new(jsonModel.User, client);
}
