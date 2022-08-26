using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildTemplate
{
    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; init; }

    [JsonPropertyName("creator_id")]
    public Snowflake CreatorId { get; init; }

    [JsonPropertyName("creator")]
    public JsonUser Creator { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    [JsonPropertyName("source_guild_id")]
    public Snowflake SourceGuildId { get; init; }

    [JsonPropertyName("serialized_source_guild")]
    public JsonGuild SerializedSourceGuild { get; init; }

    [JsonPropertyName("is_dirty")]
    public bool? IsDirty { get; init; }
}
