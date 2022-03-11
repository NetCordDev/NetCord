using System.Text.Json.Serialization;

namespace NetCord;

public class GroupDMChannelProperties
{
    public GroupDMChannelProperties(IEnumerable<string> accessTokens)
    {
        AccessTokens = accessTokens;
    }

    [JsonPropertyName("access_tokens")]
    public IEnumerable<string> AccessTokens { get; }

    //[JsonConverter(typeof(JsonConverters.GroupDMChannelPropertiesNicknamesConverter))]
    [JsonPropertyName("nicks")]
    public IReadOnlyDictionary<DiscordId, string>? Nicknames { get; set; }
}