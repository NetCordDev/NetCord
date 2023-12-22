using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageSticker : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("format_type")]
    public StickerFormat Format { get; set; }
}
