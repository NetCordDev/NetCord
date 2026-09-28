using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class PingWebhookEventArgs(JsonWebhookEventArgs jsonModel) : IWebhookEventArgs
{
    public int Version { get; } = jsonModel.Version;

    public ulong ApplicationId { get; } = jsonModel.ApplicationId;
}
