using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildThreadListSyncEventArgs
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("channel_ids")]
    public ulong[]? ChannelIds { get; set; }

    [JsonPropertyName("threads")]
    public required JsonChannel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required JsonThreadUser[] Users { get; set; }
}
