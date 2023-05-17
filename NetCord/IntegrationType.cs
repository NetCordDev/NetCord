using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.IntegrationTypeConverter))]
public enum IntegrationType
{
    Twitch,
    YouTube,
    Discord,
    GuildSubscription,
}
