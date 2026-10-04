using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Gateway.JsonModels.EventArgs;

[JsonGuard]
public partial class JsonMessageDeleteBulkEventArgs
{
    [JsonPropertyName("ids")]
    public ulong[] MessageIds { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }
}
