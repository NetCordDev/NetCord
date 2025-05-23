using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonApplicationEmojisResult
{
    [JsonPropertyName("items")]
    public JsonEmoji[] Items { get; set; }
}
