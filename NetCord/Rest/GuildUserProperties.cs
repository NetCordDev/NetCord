using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildUserProperties
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nick")]
    public string? Nickname { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("roles")]
    public IEnumerable<ulong>? RolesIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mute")]
    public bool? Muted { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deaf")]
    public bool? Deafened { get; set; }

    public GuildUserProperties(string accessToken)
    {
        AccessToken = accessToken;
    }

    [JsonSerializable(typeof(GuildUserProperties))]
    public partial class GuildUserPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildUserPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
