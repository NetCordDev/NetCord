using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildTemplate
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; set; }

    [JsonPropertyName("creator_id")]
    public ulong CreatorId { get; set; }

    [JsonPropertyName("creator")]
    public JsonUser Creator { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("source_guild_id")]
    public ulong SourceGuildId { get; set; }

    [JsonPropertyName("serialized_source_guild")]
    public JsonGuild SerializedSourceGuild { get; set; }

    [JsonPropertyName("is_dirty")]
    public bool? IsDirty { get; set; }

    [JsonSerializable(typeof(JsonGuildTemplate))]
    public partial class JsonGuildTemplateSerializerContext : JsonSerializerContext
    {
        public static JsonGuildTemplateSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonGuildTemplate[]))]
    public partial class JsonGuildTemplateArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildTemplateArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
