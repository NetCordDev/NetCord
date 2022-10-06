using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildStickerOptions
{
    internal GuildStickerOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    [JsonSerializable(typeof(GuildStickerOptions))]
    public partial class GuildStickerOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildStickerOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
