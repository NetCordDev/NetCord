using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonGuildInviteDeleteEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake InviteChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("code")]
    public string InviteCode { get; init; }
}
