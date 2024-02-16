using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildEmojiProperties(string name, ImageProperties image)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("image")]
    public ImageProperties Image { get; set; } = image;

    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? AllowedRoles { get; set; }
}
