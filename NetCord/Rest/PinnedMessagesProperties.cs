using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class PinnedMessagesProperties(DateTimeOffset? before, ulong? limit)
{
    [JsonPropertyName("before")]
    public DateTimeOffset? Before { get; set; } = before;

    [JsonPropertyName("limit")]
    public ulong? Limit { get; set; } = limit;
}
