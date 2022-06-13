using System.Text.Json.Serialization;

namespace NetCord.JsonModels;
public record JsonVoiceState
{
    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    //[JsonPropertyName("member")]
    //public GuildUser? User { get; init; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; init; }

    [JsonPropertyName("deaf")]
    public bool IsDeafened { get; init; }

    [JsonPropertyName("mute")]
    public bool IsMuted { get; init; }

    [JsonPropertyName("self_deaf")]
    public bool IsSelfDeafened { get; init; }

    [JsonPropertyName("self_mute")]
    public bool IsSelfMuted { get; init; }

    [JsonPropertyName("self_stream")]
    public bool? SelfStreamExists { get; init; }

    [JsonPropertyName("self_video")]
    public bool SelfVideoExists { get; init; }

    [JsonPropertyName("suppress")]
    public bool Suppressed { get; init; }

    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; init; }
}
