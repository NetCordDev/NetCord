namespace NetCord;

public class GuildScheduledEventUser
{
    private readonly JsonModels.JsonGuildScheduledEventUser _jsonEntity;

    public DiscordId ScheduledEventId => _jsonEntity.ScheduledEventId;

    public User User { get; }

    internal GuildScheduledEventUser(JsonModels.JsonGuildScheduledEventUser jsonEntity, DiscordId guildId, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.GuildUser != null)
        {
            User = new GuildUser(jsonEntity.GuildUser with
            {
                User = jsonEntity.User
            }, guildId, client);
        }
        else
            User = new(jsonEntity.User, client);
    }
}