using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonMessagePin
{
    [JsonPropertyName("pinned_at")]
    public DateTimeOffset PinnedAt { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage Message { get; set; }
}
