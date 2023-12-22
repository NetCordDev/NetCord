using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonClientDisconnect
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
}
