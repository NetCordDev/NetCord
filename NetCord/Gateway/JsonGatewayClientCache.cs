using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway;

public class JsonGatewayClientCache
{
    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("dm_channels")]
    public IReadOnlyDictionary<ulong, JsonChannel> DMChannels { get; set; }

    [JsonPropertyName("group_dm_channels")]
    public IReadOnlyDictionary<ulong, JsonChannel> GroupDMChannels { get; set; }

    [JsonPropertyName("guilds")]
    public IReadOnlyDictionary<ulong, JsonGuild> Guilds { get; set; }
}
