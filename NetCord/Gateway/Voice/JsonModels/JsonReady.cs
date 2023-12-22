using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonReady
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }

    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("port")]
    public ushort Port { get; set; }

    [JsonPropertyName("modes")]
    public string[] Modes { get; set; }
}
