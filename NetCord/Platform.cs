using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Platform
    {
        Desktop,
        Mobile,
        Web,
    }
}