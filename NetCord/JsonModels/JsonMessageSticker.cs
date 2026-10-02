using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageSticker : JsonEntity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("format_type")]
    public required StickerFormat Format { get; set; }
}
