using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents a user subscribed to a guild scheduled event.
/// </summary>
public class GuildScheduledEventUser : IJsonModel<JsonGuildScheduledEventUser>
{
    JsonGuildScheduledEventUser IJsonModel<JsonGuildScheduledEventUser>.JsonModel => _jsonModel;
    private readonly JsonGuildScheduledEventUser _jsonModel;

    /// <summary>
    /// The ID of the scheduled event.
    /// </summary>
    public ulong ScheduledEventId => _jsonModel.ScheduledEventId;

    /// <summary>
    /// The user subscribed to the scheduled event.
    /// </summary>
    public User User { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildScheduledEventUser"/> class.
    /// </summary>
    public GuildScheduledEventUser(JsonGuildScheduledEventUser jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;

        if (jsonModel.GuildUser is { } guildUser)
        {
            guildUser.User = jsonModel.User;
            User = new GuildUser(guildUser, guildId, client);
        }
        else
        {
            User = new User(jsonModel.User, client);
        }
    }
}
