using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

public class JsonVoiceClientCache
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }

    [JsonPropertyName("ssrcs")]
    public ImmutableDictionary<ulong, uint> Ssrcs { get; set; }

    [JsonPropertyName("users")]
    public ImmutableDictionary<uint, ulong> Users { get; set; }
}
