using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMessageSticker : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("format_type")]
    public StickerFormat Format { get; init; }
}
