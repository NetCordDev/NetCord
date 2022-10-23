using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ForumTagProperties
{
    public ForumTagProperties(string name)
    {
        Name = name;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("moderated")]
    public bool? Moderated { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }

    [JsonSerializable(typeof(ForumTagProperties))]
    public partial class ForumTagPropertiesSerializerContext : JsonSerializerContext
    {
        public static ForumTagPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
