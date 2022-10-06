using System.Text.Json.Serialization;

namespace NetCord.JsonModels;
public partial class JsonVoiceState
{
    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; set; }

    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; set; }

    //[JsonPropertyName("member")]
    //public GuildUser? User { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    [JsonPropertyName("deaf")]
    public bool IsDeafened { get; set; }

    [JsonPropertyName("mute")]
    public bool IsMuted { get; set; }

    [JsonPropertyName("self_deaf")]
    public bool IsSelfDeafened { get; set; }

    [JsonPropertyName("self_mute")]
    public bool IsSelfMuted { get; set; }

    [JsonPropertyName("self_stream")]
    public bool? SelfStreamExists { get; set; }

    [JsonPropertyName("self_video")]
    public bool SelfVideoExists { get; set; }

    [JsonPropertyName("suppress")]
    public bool Suppressed { get; set; }

    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }

    [JsonSerializable(typeof(JsonVoiceState))]
    public partial class JsonVoiceStateSerializerContext : JsonSerializerContext
    {
        public static JsonVoiceStateSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
