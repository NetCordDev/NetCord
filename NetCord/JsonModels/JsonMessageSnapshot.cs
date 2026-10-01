using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageSnapshot
{
    [JsonPropertyName("message")]
    public required JsonMessageSnapshotMessage Message { get; set; }
}
