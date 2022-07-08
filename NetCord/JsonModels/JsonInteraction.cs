using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonInteraction : JsonEntity
{
    [JsonPropertyName("application_id")]
    public Snowflake ApplicationId { get; init; }

    [JsonPropertyName("type")]
    public InteractionType Type { get; init; }

    [JsonPropertyName("data")]
    public JsonInteractionData Data { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; init; }

    [JsonPropertyName("token")]
    public string Token { get; init; }

    [JsonPropertyName("message")]
    public JsonMessage Message { get; init; }

    [JsonPropertyName("app_permissions")]
    public string? AppPermissions { get; init; }

    [JsonPropertyName("locale")]
    public CultureInfo UserLocale { get; init; }

    [JsonPropertyName("guild_locale")]
    public CultureInfo GuildLocale { get; init; }
}