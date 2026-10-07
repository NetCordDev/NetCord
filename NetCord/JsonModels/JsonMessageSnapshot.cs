using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessageSnapshot
{
    [JsonPropertyName("message")]
    public JsonMessageSnapshotMessage Message { get; set; }
}
