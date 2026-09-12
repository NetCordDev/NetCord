using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonMessagePin
{
    [JsonPropertyName("pinned_at")]
    public DateTimeOffset PinnedAt { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage Message { get; set; }
}
