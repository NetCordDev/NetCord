using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public partial struct MentionableValueProperties(ulong id, MentionableValueType type)
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

    [JsonConverter(typeof(StringEnumConverterWithErrorHandling<MentionableValueType>))]
    [JsonPropertyName("type")]
    public MentionableValueType Type { get; set; } = type;
}
