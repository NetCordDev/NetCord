using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonVoiceChannelEffectSendEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("animation_type")]
    public VoiceChannelEffectSendAnimationType? AnimationType { get; set; }

    [JsonPropertyName("animation_id")]
    public ulong? AnimationId { get; set; }

    [JsonPropertyName("sound_id")]
    public ulong? SoundId { get; set; }

    [JsonPropertyName("sound_volume")]
    public double? SoundVolume { get; set; }
}
