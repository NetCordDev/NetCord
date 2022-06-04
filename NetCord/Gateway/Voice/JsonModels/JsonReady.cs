using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal record JsonReady
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; init; }

    [JsonPropertyName("ip")]
    public string Ip { get; init; }

    [JsonPropertyName("port")]
    public int Port { get; init; }

    [JsonPropertyName("modes")]
    public string[] Modes { get; init; }
}
