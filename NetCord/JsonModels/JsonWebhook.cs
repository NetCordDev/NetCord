using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonWebhook : JsonEntity
{
    [JsonPropertyName("type")]
    public WebhookType Type { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("avatar")]
    public string? AvatarHash { get; init; }

    [JsonPropertyName("token")]
    public string? Token { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }

    [JsonPropertyName("source_guild")]
    public RestGuild? Guild { get; init; }

    [JsonPropertyName("source_channel")]
    public string? Url { get; init; }
}