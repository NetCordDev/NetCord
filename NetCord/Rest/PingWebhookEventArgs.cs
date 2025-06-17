using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class PingWebhookEventArgs(JsonWebhookEventArgs jsonModel) : IWebhookEventArgs
{
    JsonWebhookEventArgs IJsonModel<JsonWebhookEventArgs>.JsonModel => jsonModel;

    public int Version => jsonModel.Version;

    public ulong ApplicationId => jsonModel.ApplicationId;
}
