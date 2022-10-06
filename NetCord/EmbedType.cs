using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<EmbedType>))]
public enum EmbedType
{
    Rich,
    Image,
    Video,
    Gifv,
    Article,
    Link,
}
