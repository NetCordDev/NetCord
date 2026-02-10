using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

public class JsonVoiceClientCache
{
    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }

    [JsonPropertyName("users")]
    public IReadOnlyList<ulong> Users { get; set; }

    [JsonPropertyName("user_ssrcs")]
    public IReadOnlyDictionary<ulong, uint> UserSsrcs { get; set; }

    [JsonPropertyName("ssrc_users")]
    public IReadOnlyDictionary<uint, ulong> SsrcUsers { get; set; }
}
