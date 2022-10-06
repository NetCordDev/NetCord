using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class VoiceStateOptions
{
    internal VoiceStateOptions(Snowflake channelId)
    {
        ChannelId = channelId;
    }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }

    [JsonSerializable(typeof(VoiceStateOptions))]
    public partial class VoiceStateOptionsSerializerContext : JsonSerializerContext
    {
        public static VoiceStateOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
