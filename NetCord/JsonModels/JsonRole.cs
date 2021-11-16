using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonRole : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("color")]
    public Color Color { get; init; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; init; }

    [JsonPropertyName("position")]
    public int Position { get; init; }

    [JsonPropertyName("permissions")]
    public string Permissions { get; init; }

    [JsonPropertyName("managed")]
    public bool Managed { get; init; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; init; }

    [JsonPropertyName("tags")]
    public JsonTags? Tags { get; init; }
}

internal record JsonTags
{
    [JsonPropertyName("bot_id")]
    public DiscordId? BotId { get; init; }

    [JsonPropertyName("integration_id")]
    public DiscordId? IntegrationId { get; init; }
    [JsonPropertyName("premium_subscriber")]
    public bool IsPremiumSubscriber { get; init; }
}