using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmoji
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("roles")]
    public JsonRole[] AllowedRoles { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; set; }

    [JsonPropertyName("require_colons")]
    public bool? RequireColons { get; set; }

    [JsonPropertyName("managed")]
    public bool? Managed { get; set; }

    [JsonPropertyName("animated")]
    public bool Animated { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }

    [JsonSerializable(typeof(JsonEmoji))]
    public partial class JsonEmojiSerializerContext : JsonSerializerContext
    {
        public static JsonEmojiSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonEmoji[]))]
    public partial class JsonEmojiArraySerializerContext : JsonSerializerContext
    {
        public static JsonEmojiArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
