using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildEmojiProperties
{
    public GuildEmojiProperties(string name, ImageProperties image)
    {
        Name = name;
        Image = image;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("image")]
    public ImageProperties Image { get; set; }

    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? AllowedRoles { get; set; }

    [JsonSerializable(typeof(GuildEmojiProperties))]
    public partial class GuildEmojiPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildEmojiPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
