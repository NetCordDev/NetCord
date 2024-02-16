using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GroupDMChannelProperties(IEnumerable<string> accessTokens)
{
    [JsonPropertyName("access_tokens")]
    public IEnumerable<string> AccessTokens { get; set; } = accessTokens;

    [JsonPropertyName("nicks")]
    public IReadOnlyDictionary<ulong, string>? Nicknames { get; set; }
}
