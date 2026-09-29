using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonChannelPins
{
    [JsonPropertyName("items")]
    public required JsonMessagePin[] Items { get; set; }

    [JsonPropertyName("has_more")]
    public required bool HasMore { get; set; }
}
