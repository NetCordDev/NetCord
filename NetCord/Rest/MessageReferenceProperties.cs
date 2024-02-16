using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageReferenceProperties(ulong messageId, bool failIfNotExists = true)
{
    [JsonPropertyName("message_id")]
    public ulong Id { get; set; } = messageId;

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; set; } = failIfNotExists;
}
