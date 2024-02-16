namespace NetCord.Gateway;

public class WebhooksUpdateEventArgs(JsonModels.EventArgs.JsonWebhooksUpdateEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonWebhooksUpdateEventArgs>
{
    JsonModels.EventArgs.JsonWebhooksUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonWebhooksUpdateEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public ulong ChannelId => jsonModel.ChannelId;
}
