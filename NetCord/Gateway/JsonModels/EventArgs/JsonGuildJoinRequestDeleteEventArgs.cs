using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildJoinRequestDeleteEventArgs
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }
    
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
