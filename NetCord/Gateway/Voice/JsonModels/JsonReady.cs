using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Gateway.Voice.JsonModels;

[JsonGuard]
internal partial class JsonReady
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
