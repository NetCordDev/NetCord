using System.Text.Json.Serialization;

using NetCord.Gateway.JsonModels;

namespace NetCord.JsonModels;

public class JsonThreadUser : JsonThreadCurrentUser
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("id")]
    public ulong ThreadId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("presence")]
    public JsonPresence? Presence { get; set; }
}
