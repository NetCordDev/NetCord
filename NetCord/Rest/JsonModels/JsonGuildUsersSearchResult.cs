using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonGuildUsersSearchResult
{
    [JsonPropertyName("members")]
    public JsonGuildUserInfo[] Users { get; set; }
}
