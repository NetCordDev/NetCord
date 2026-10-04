using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonVoiceState
{
    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; set; }

    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }

    [JsonPropertyName("deaf")]
    public required bool Deaf { get; set; }

    [JsonPropertyName("mute")]
    public required bool Mute { get; set; }

    [JsonPropertyName("self_deaf")]
    public required bool SelfDeaf { get; set; }

    [JsonPropertyName("self_mute")]
    public required bool SelfMute { get; set; }

    [JsonPropertyName("self_stream")]
    public bool? SelfStream { get; set; }

    [JsonPropertyName("self_video")]
    public required bool SelfVideo { get; set; }

    [JsonPropertyName("suppress")]
    public required bool Suppress { get; set; }

    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }
}
