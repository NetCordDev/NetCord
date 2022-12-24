using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class BulkDeleteMessagesProperties
{
    public BulkDeleteMessagesProperties(ulong[] messageIds)
    {
        MessageIds = messageIds;
    }

    [JsonPropertyName("messages")]
    public ulong[] MessageIds { get; }

    [JsonSerializable(typeof(BulkDeleteMessagesProperties))]
    public partial class BulkDeleteMessagesPropertiesSerializerContext : JsonSerializerContext
    {
        public static BulkDeleteMessagesPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
