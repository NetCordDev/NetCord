using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonThreadListSyncEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("channel_ids")]
    public Snowflake[]? ChannelIds { get; init; }

    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; init; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; init; }
}
