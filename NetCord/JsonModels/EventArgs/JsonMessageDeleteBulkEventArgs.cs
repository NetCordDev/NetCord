using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonMessageDeleteBulkEventArgs
{
    [JsonPropertyName("ids")]
    public Snowflake[] MessageIds { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }
}