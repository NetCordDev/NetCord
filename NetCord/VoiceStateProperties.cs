using System.Text.Json.Serialization;

namespace NetCord;

public class VoiceStateProperties
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_mute")]
    public bool? SelfMute { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_deaf")]
    public bool? SelfDeaf { get; set; }

    public VoiceStateProperties(DiscordId guildId, DiscordId? channelId)
    {
        GuildId = guildId;
        ChannelId = channelId;
    }
}