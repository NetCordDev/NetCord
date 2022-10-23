using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageReferenceProperties : Entity
{
    [JsonPropertyName("message_id")]
    public override ulong Id { get; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; }

    public MessageReferenceProperties(ulong messageId, bool failIfNotExists = true)
    {
        Id = messageId;
        FailIfNotExists = failIfNotExists;
    }

    [JsonSerializable(typeof(MessageReferenceProperties))]
    public partial class MessageReferencePropertiesSerializerContext : JsonSerializerContext
    {
        public static MessageReferencePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
