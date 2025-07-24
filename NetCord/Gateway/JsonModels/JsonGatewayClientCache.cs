using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonGatewayClientCache
{
    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("guilds")]
    public JsonGuild[] Guilds { get; set; }
}
