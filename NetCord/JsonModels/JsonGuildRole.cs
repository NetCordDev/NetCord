using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildRole : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("color")]
    public Color Color { get; init; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; init; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; init; }

    [JsonPropertyName("unicode_emoji")]
    public string? UnicodeEmoji { get; init; }

    [JsonPropertyName("position")]
    public int Position { get; init; }

    [JsonPropertyName("permissions")]
    public Permission Permissions { get; init; }

    [JsonPropertyName("managed")]
    public bool Managed { get; init; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; init; }

    [JsonPropertyName("tags")]
    public JsonTags? Tags { get; init; }
}

public record JsonTags
{
    [JsonPropertyName("bot_id")]
    public Snowflake? BotId { get; init; }

    [JsonPropertyName("integration_id")]
    public Snowflake? IntegrationId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.NullConverter))]
    [JsonPropertyName("premium_subscriber")]
    public bool IsPremiumSubscriber { get; init; }
}