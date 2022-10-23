namespace NetCord.Gateway;

public class WebhooksUpdateEventArgs : IJsonModel<JsonModels.EventArgs.JsonWebhooksUpdateEventArgs>
{
    JsonModels.EventArgs.JsonWebhooksUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonWebhooksUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonWebhooksUpdateEventArgs _jsonModel;

    public WebhooksUpdateEventArgs(JsonModels.EventArgs.JsonWebhooksUpdateEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong GuildId => _jsonModel.GuildId;

    public ulong ChannelId => _jsonModel.ChannelId;
}
