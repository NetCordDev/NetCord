using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageReferenceProperties
{
    public static MessageReferenceProperties Reply(ulong messageId, bool failIfNotExists = true)
    {
        return new()
        {
            Type = MessageReferenceType.Reply,
            MessageId = messageId,
            FailIfNotExists = failIfNotExists,
        };
    }

    public static MessageReferenceProperties Forward(ulong channelId, ulong messageId, bool failIfNotExists = true)
    {
        return new()
        {
            Type = MessageReferenceType.Forward,
            ChannelId = channelId,
            MessageId = messageId,
            FailIfNotExists = failIfNotExists,
        };
    }

    private MessageReferenceProperties()
    {
    }

    [JsonPropertyName("type")]
    public MessageReferenceType Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool FailIfNotExists { get; set; }
}
