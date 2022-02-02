using System.Text.Json.Serialization;

namespace NetCord;

public class MessageReferenceProperties : Entity
{
    [JsonPropertyName("message_id")]
    public override DiscordId Id { get; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; }

    public MessageReferenceProperties(DiscordId messageId, bool failIfNotExists = true)
    {
        Id = messageId;
        FailIfNotExists = failIfNotExists;
    }
}
