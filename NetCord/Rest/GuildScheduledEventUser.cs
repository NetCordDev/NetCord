using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildScheduledEventUser(JsonGuildScheduledEventUser jsonModel, ulong guildId, RestClient client)
{
    public ulong ScheduledEventId { get; } = jsonModel.ScheduledEventId;

    public User User { get; } = jsonModel.GuildUser is { } guildUser ? new GuildUser(guildUser, guildId, client) : new User(jsonModel.User, client);
}
