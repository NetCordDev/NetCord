using System.Text.Json.Serialization;

namespace NetCord;

public class GuildEmojiProperties
{
    public GuildEmojiProperties(string name, ImageProperties image)
    {
        Name = name;
        Image = image;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("image")]
    public ImageProperties Image { get; }

    [JsonPropertyName("roles")]
    public IEnumerable<Snowflake>? AllowedRoles { get; set; }
}