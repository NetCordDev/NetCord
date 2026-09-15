using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents a user's presence update in a guild.
/// </summary>
public class Presence(JsonModels.JsonPresence jsonModel, ulong? guildId, RestClient client) : IJsonModel<JsonModels.JsonPresence>
{
    JsonModels.JsonPresence IJsonModel<JsonModels.JsonPresence>.JsonModel => jsonModel;

    /// <summary>
    /// The user associated with the presence update.
    /// </summary>
    public User User { get; } = new(jsonModel.User, client);

    /// <summary>
    /// The ID of the guild the presence update occurred in.
    /// </summary>
    public ulong GuildId { get; } = guildId ?? jsonModel.GuildId.GetValueOrDefault();

    /// <summary>
    /// The user's status.
    /// </summary>
    public UserStatusType Status => jsonModel.Status;

    /// <summary>
    /// The user's current activities.
    /// </summary>
    public IReadOnlyList<UserActivity> Activities { get; }
        = jsonModel.Activities.SelectOrEmpty(
            a => new UserActivity(a, guildId ?? jsonModel.GuildId.GetValueOrDefault(), client)
        ).ToArray();

    /// <summary>
    /// The user's status per active platform.
    /// </summary>
    public IReadOnlyDictionary<Platform, UserStatusType> Platform => jsonModel.Platform;
}
