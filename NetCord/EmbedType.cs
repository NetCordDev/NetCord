using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling))]
public enum EmbedType
{
    Rich,
    Image,
    Video,
    Gifv,
    Article,
    Link,
}