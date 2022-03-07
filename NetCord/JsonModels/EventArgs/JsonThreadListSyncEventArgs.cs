using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonThreadListSyncEventArgs
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("channel_ids")]
    public DiscordId[]? ChannelIds { get; init; }

    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; init; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; init; }
}
