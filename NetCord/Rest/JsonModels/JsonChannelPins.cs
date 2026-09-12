using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonChannelPins
{
    [JsonPropertyName("items")]
    public JsonMessagePin[] Items { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
