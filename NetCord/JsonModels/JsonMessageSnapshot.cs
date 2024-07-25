using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageSnapshot
{
    [JsonPropertyName("message")]
    public JsonMessageSnapshotMessage Message { get; set; }
}
