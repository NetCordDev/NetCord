using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<IntegrationType>))]
public enum IntegrationType
{
    [JsonPropertyName("twitch")]
    Twitch,

    [JsonPropertyName("youtube")]
    YouTube,

    [JsonPropertyName("discord")]
    Discord,

    [JsonPropertyName("guild_subscription")]
    GuildSubscription,
}
