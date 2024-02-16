using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class VoiceStateProperties(ulong guildId, ulong? channelId)
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; } = guildId;

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; } = channelId;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_mute")]
    public bool? SelfMute { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_deaf")]
    public bool? SelfDeaf { get; set; }
}
