using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageEmbedType
    {
        Rich,
        Image,
        Video,
        Gifv,
        Article,
        Link,
    }
}