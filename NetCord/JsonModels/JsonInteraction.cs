using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonInteraction : JsonEntity
{
    [JsonPropertyName("application_id")]
    public DiscordId ApplicationId { get; init; }

    [JsonPropertyName("type")]
    public InteractionType Type { get; init; }

    [JsonPropertyName("data")]
    public JsonInteractionData Data { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; init; }

    [JsonPropertyName("token")]
    public string Token { get; init; }

    [JsonPropertyName("message")]
    public JsonMessage Message { get; init; }

    [JsonConverter(typeof(JsonConverters.CultureInfoConverter))]
    [JsonPropertyName("locale")]
    public CultureInfo UserLocale { get; init; }

    [JsonConverter(typeof(JsonConverters.CultureInfoConverter))]
    [JsonPropertyName("guild_locale")]
    public CultureInfo GuildLocale { get; init; }
}
