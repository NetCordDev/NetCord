using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EmbedType
    {
        Rich,
        Image,
        Video,
        Gifv,
        Article,
        Link,
    }
}