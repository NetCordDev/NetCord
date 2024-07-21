using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<EmbedType>))]
public enum EmbedType
{
    [JsonPropertyName("rich")]
    Rich,

    [JsonPropertyName("image")]
    Image,

    [JsonPropertyName("video")]
    Video,

    [JsonPropertyName("gifv")]
    Gifv,

    [JsonPropertyName("article")]
    Article,

    [JsonPropertyName("link")]
    Link,
}
