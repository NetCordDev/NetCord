using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public interface IWebhookEventArgs : IJsonModel<JsonWebhookEventArgs>
{
    public int Version { get; }

    public ulong ApplicationId { get; }
}
