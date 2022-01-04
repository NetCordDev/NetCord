using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IntegrationType
{
    Twitch,
    YouTube,
    Discord
}