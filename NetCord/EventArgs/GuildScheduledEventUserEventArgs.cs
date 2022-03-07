using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildScheduledEventUserEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs _jsonEntity;

    internal GuildScheduledEventUserEventArgs(JsonGuildScheduledEventUserEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId GuildScheduledEventId => _jsonEntity.GuildScheduledEventId;

    public DiscordId UserId => _jsonEntity.UserId;

    public DiscordId GuildId => _jsonEntity.GuildId;
}
