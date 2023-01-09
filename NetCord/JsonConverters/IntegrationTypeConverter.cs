using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonConverters;

internal class IntegrationTypeConverter : JsonConverter<IntegrationType>
{
    public override IntegrationType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "twitch" => IntegrationType.Twitch,
            "youtube" => IntegrationType.YouTube,
            "discord" => IntegrationType.Discord,
            "guild_subscription" => IntegrationType.GuildSubscription,
            _ => (IntegrationType)(-1),
        };
    }

    public override void Write(Utf8JsonWriter writer, IntegrationType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            IntegrationType.Twitch => "twitch",
            IntegrationType.YouTube => "youtube",
            IntegrationType.Discord => "discord",
            IntegrationType.GuildSubscription => "guild_subscription",
            _ => string.Empty,
        });
    }
}
