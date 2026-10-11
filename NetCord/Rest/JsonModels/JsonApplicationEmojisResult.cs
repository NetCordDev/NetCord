using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonApplicationEmojisResult
{
    [JsonPropertyName("items")]
    public JsonEmoji[] Items { get; set; }
}
