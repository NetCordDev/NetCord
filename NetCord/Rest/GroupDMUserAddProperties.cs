using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class GroupDMChannelUserAddProperties(string accessToken)
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = accessToken;

    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }
}
