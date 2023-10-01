using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public readonly partial struct MentionableValueProperties : IEntity
{
    public MentionableValueProperties(ulong id, MentionableValueType type)
    {
        Id = id;
        Type = type;
    }

    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonConverter(typeof(StringEnumConverterWithErrorHandling<MentionableValueType>))]
    [JsonPropertyName("type")]
    public MentionableValueType Type { get; }
}
