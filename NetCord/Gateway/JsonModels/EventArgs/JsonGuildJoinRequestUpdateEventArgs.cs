using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildJoinRequestUpdateEventArgs
{
    [JsonPropertyName("status")]
    public GuildJoinRequestStatus Status { get; set; }
    [JsonPropertyName("request")]
    public JsonGuildJoinRequest Request { get; set; }
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
