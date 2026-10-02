using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonMessagePin
{
    [JsonPropertyName("pinned_at")]
    public required DateTimeOffset PinnedAt { get; set; }

    [JsonPropertyName("message")]
    public required JsonMessage Message { get; set; }
}
