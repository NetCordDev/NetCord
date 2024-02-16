using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class BulkDeleteMessagesProperties(ArraySegment<ulong> messageIds)
{
    [JsonPropertyName("messages")]
    public ArraySegment<ulong> MessageIds { get; set; } = messageIds;
}
