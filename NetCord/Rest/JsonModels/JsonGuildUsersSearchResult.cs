using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGuildUsersSearchResult
{
    [JsonPropertyName("members")]
    public JsonGuildUserInfo[] Users { get; set; }
}
