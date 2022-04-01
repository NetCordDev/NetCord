using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonSticker : JsonEntity
{
    [JsonPropertyName("pack_id")]
    public Snowflake? PackId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("tags")]
    public string Tags { get; init; }

    [JsonPropertyName("type")]
    public StickerType Type { get; init; }

    [JsonPropertyName("format_type")]
    public StickerFormat Format { get; init; }

    [JsonPropertyName("available")]
    public bool? Available { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; init; }

    [JsonPropertyName("sort_value")]
    public int SortValue { get; init; }
}

internal enum StickerType
{
    Standard = 1,
    Guild = 2,
}