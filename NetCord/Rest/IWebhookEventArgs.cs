using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public interface IWebhookEventArgs : IJsonModel<JsonWebhookEventArgs>
{
    public int Version { get; }

    public ulong ApplicationId { get; }

    public static IWebhookEventArgs CreateFromJson(JsonWebhookEventArgs jsonModel, RestClient client)
    {
        return jsonModel.Type switch
        {
            WebhookEventType.Ping => new PingWebhookEventArgs(jsonModel),
            WebhookEventType.Event => WebhookEventArgs.CreateFromJson(jsonModel, client),
            _ => throw new InvalidOperationException($"Unknown webhook event type: {jsonModel.Type}."),
        };
    }
}
