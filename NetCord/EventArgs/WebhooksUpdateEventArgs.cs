namespace NetCord;

public class WebhooksUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonWebhooksUpdateEventArgs _jsonEntity;

    internal WebhooksUpdateEventArgs(JsonModels.EventArgs.JsonWebhooksUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId GuildId => _jsonEntity.GuildId;

    public DiscordId ChannelId => _jsonEntity.ChannelId;
}
