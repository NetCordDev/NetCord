using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GroupDMChannelProperties
{
    public GroupDMChannelProperties(IEnumerable<string> accessTokens)
    {
        AccessTokens = accessTokens;
    }

    [JsonPropertyName("access_tokens")]
    public IEnumerable<string> AccessTokens { get; set; }

    [JsonPropertyName("nicks")]
    public IReadOnlyDictionary<ulong, string>? Nicknames { get; set; }

    [JsonSerializable(typeof(GroupDMChannelProperties))]
    public partial class GroupDMChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GroupDMChannelPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
