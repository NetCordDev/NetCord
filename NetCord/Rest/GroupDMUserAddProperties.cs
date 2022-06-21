using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GroupDMUserAddProperties
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; }

    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }

    public GroupDMUserAddProperties(string accessToken)
    {
        AccessToken = accessToken;
    }
}