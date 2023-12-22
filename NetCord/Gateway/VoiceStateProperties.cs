using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class VoiceStateProperties
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_mute")]
    public bool? SelfMute { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_deaf")]
    public bool? SelfDeaf { get; set; }

    public VoiceStateProperties(ulong guildId, ulong? channelId)
    {
        GuildId = guildId;
        ChannelId = channelId;
    }
}
