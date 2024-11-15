using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonClientConnect
{
    [JsonPropertyName("user_ids")]
    public ulong[] UserIds { get; set; }
}
