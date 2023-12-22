using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonFollowedChannel : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public override ulong Id { get; set; }

    [JsonPropertyName("webhook_id")]
    public ulong WebhookId { get; set; }
}
