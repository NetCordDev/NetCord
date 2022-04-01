namespace NetCord;

public class GuildWidgetSettings
{
    private readonly JsonModels.JsonGuildWidgetSettings _jsonEntity;

    public bool Enabled => _jsonEntity.Enabled;

    public Snowflake? ChannelId => _jsonEntity.ChannelId;

    internal GuildWidgetSettings(JsonModels.JsonGuildWidgetSettings jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}