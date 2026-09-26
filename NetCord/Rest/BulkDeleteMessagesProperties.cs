using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class BulkDeleteMessagesProperties(ReadOnlyMemory<ulong> messageIds)
{
    [JsonPropertyName("messages")]
    public ReadOnlyMemory<ulong> MessageIds { get; set; } = messageIds;
}
