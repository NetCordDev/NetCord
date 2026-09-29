using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonApplicationEmojisResult
{
    [JsonPropertyName("items")]
    public required JsonEmoji[] Items { get; set; }
}
