using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class GuildUsersRequestProperties(ulong guildId)
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; } = guildId;

    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("presences")]
    public bool? Presences { get; set; }

    [JsonPropertyName("user_ids")]
    public IEnumerable<ulong>? UserIds { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }
}
