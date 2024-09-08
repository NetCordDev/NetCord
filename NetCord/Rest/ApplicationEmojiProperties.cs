using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationEmojiProperties(string name, ImageProperties image)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("image")]
    public ImageProperties Image { get; set; } = image;
}
