using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonGatewayClientCache
{
    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("dm_channels")]
    public IReadOnlyList<JsonChannel> DMChannels { get; set; }

    [JsonPropertyName("guilds")]
    public IReadOnlyList<JsonGuild> Guilds { get; set; }
}
