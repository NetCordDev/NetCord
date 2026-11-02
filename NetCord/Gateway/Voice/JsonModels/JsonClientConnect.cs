using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Gateway.Voice.JsonModels;

[JsonGuard]
internal partial class JsonClientConnect
{
    [JsonPropertyName("user_ids")]
    public ulong[] UserIds { get; set; }
}
