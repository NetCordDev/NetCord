using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GroupDMChannelProperties
{
    public GroupDMChannelProperties(IEnumerable<string> accessTokens)
    {
        AccessTokens = accessTokens;
    }

    [JsonPropertyName("access_tokens")]
    public IEnumerable<string> AccessTokens { get; }

    [JsonPropertyName("nicks")]
    public IReadOnlyDictionary<ulong, string>? Nicknames { get; set; }

    [JsonSerializable(typeof(GroupDMChannelProperties))]
    public partial class GroupDMChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GroupDMChannelPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
