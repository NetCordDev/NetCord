namespace NetCord;

public class WebhooksUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonWebhooksUpdateEventArgs _jsonEntity;

    internal WebhooksUpdateEventArgs(JsonModels.EventArgs.JsonWebhooksUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public Snowflake ChannelId => _jsonEntity.ChannelId;
}
