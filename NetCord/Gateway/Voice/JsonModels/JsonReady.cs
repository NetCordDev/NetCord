using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

public record JsonReady
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; init; }

    [JsonPropertyName("ip")]
    public string Ip { get; init; }

    [JsonPropertyName("port")]
    public ushort Port { get; init; }

    [JsonPropertyName("modes")]
    public string[] Modes { get; init; }
}
