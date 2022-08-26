using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public class GuildUsersRequestProperties
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("presences")]
    public bool? Presences { get; set; }

    [JsonPropertyName("user_ids")]
    public IEnumerable<Snowflake>? UserIds { get; set; }

    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    public GuildUsersRequestProperties(Snowflake guildId)
    {
        GuildId = guildId;
    }
}
