using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildTemplate
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("usage_count")]
    public required int UsageCount { get; set; }

    [JsonPropertyName("creator_id")]
    public required ulong CreatorId { get; set; }

    [JsonPropertyName("creator")]
    public required JsonUser Creator { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public required DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("source_guild_id")]
    public required ulong SourceGuildId { get; set; }

    [JsonPropertyName("serialized_source_guild")]
    public required JsonGuildTemplateSerializedSourceGuild SerializedSourceGuild { get; set; }

    [JsonPropertyName("is_dirty")]
    public bool? IsDirty { get; set; }
}
