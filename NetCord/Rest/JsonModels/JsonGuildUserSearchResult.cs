using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGuildUserSearchResult
{
    [JsonPropertyName("members")]
    public JsonGuildUserInfo[] Users { get; set; }
}
