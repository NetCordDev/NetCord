using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public partial class JsonVoiceState
{
    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; set; }

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
        public static JsonVoiceStateSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
