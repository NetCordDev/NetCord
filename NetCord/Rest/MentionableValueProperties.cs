using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial struct MentionableValueProperties(ulong id, MentionableValueType type)
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

    [JsonPropertyName("type")]
    public MentionableValueType Type { get; set; } = type;
}
