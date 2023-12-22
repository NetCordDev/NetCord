using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class BulkDeleteMessagesProperties
{
    public BulkDeleteMessagesProperties(ArraySegment<ulong> messageIds)
    {
        MessageIds = messageIds;
    }

    [JsonPropertyName("messages")]
    public ArraySegment<ulong> MessageIds { get; set; }
}
