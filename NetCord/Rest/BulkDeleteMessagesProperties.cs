using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class BulkDeleteMessagesProperties
{
    public BulkDeleteMessagesProperties(ArraySegment<ulong> messageIds)
    {
        MessageIds = messageIds;
    }

    [JsonPropertyName("messages")]
    public ArraySegment<ulong> MessageIds { get; set; }

    [JsonSerializable(typeof(BulkDeleteMessagesProperties))]
    public partial class BulkDeleteMessagesPropertiesSerializerContext : JsonSerializerContext
    {
        public static BulkDeleteMessagesPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
