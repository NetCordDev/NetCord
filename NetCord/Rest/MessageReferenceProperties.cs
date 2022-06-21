using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class MessageReferenceProperties : Entity
{
    [JsonPropertyName("message_id")]
    public override Snowflake Id { get; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; }

    public MessageReferenceProperties(Snowflake messageId, bool failIfNotExists = true)
    {
        Id = messageId;
        FailIfNotExists = failIfNotExists;
    }
}
