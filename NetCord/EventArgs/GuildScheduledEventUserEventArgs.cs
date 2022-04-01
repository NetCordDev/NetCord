using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildScheduledEventUserEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs _jsonEntity;

    internal GuildScheduledEventUserEventArgs(JsonGuildScheduledEventUserEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake GuildScheduledEventId => _jsonEntity.GuildScheduledEventId;

    public Snowflake UserId => _jsonEntity.UserId;

    public Snowflake GuildId => _jsonEntity.GuildId;
}
