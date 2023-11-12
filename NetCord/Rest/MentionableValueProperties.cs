using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public partial struct MentionableValueProperties
{
    public MentionableValueProperties(ulong id, MentionableValueType type)
    {
        Id = id;
        Type = type;
    }

    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonConverter(typeof(StringEnumConverterWithErrorHandling<MentionableValueType>))]
    [JsonPropertyName("type")]
    public MentionableValueType Type { get; set; }
}
