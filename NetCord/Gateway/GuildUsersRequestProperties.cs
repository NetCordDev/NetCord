using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class GuildUsersRequestProperties
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

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

    public GuildUsersRequestProperties(ulong guildId)
    {
        GuildId = guildId;
    }

    [JsonSerializable(typeof(GuildUsersRequestProperties))]
    public partial class GuildUsersRequestPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildUsersRequestPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
