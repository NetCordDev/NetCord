using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonChannelPins
{
    [JsonPropertyName("items")]
    public JsonMessagePin[] Items { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
