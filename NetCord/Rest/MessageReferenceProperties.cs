using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageReferenceProperties
{
    [JsonPropertyName("message_id")]
    public ulong Id { get; set; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; set; }

    public MessageReferenceProperties(ulong messageId, bool failIfNotExists = true)
    {
        Id = messageId;
        FailIfNotExists = failIfNotExists;
    }

    [JsonSerializable(typeof(MessageReferenceProperties))]
    public partial class MessageReferencePropertiesSerializerContext : JsonSerializerContext
    {
        public static MessageReferencePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
