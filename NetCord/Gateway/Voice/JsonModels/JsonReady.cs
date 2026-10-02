using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonReady
{
    [JsonPropertyName("ssrc")]
    public required uint Ssrc { get; set; }

    [JsonPropertyName("ip")]
    public required string Ip { get; set; }

    [JsonPropertyName("port")]
    public required ushort Port { get; set; }

    [JsonPropertyName("modes")]
    public required string[] Modes { get; set; }
}
