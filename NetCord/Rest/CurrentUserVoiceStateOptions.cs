using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class CurrentUserVoiceStateOptions
{
    internal CurrentUserVoiceStateOptions()
    {
    }

    /// <summary>
    /// The id of the channel the user is currently in.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// Toggles the user's suppress state.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }

    /// <summary>
    /// Sets the user's request to speak.
    /// </summary>
    [JsonConverter(typeof(JsonConverters.NullableDateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }

    [JsonSerializable(typeof(CurrentUserVoiceStateOptions))]
    public partial class CurrentUserVoiceStateOptionsSerializerContext : JsonSerializerContext
    {
        public static CurrentUserVoiceStateOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
