using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class VoiceStateProperties
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_mute")]
    public bool? SelfMute { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("self_deaf")]
    public bool? SelfDeaf { get; set; }

    public VoiceStateProperties(Snowflake guildId, Snowflake? channelId)
    {
        GuildId = guildId;
        ChannelId = channelId;
    }

    [JsonSerializable(typeof(VoiceStateProperties))]
    public partial class VoiceStatePropertiesSerializerContext : JsonSerializerContext
    {
        public static VoiceStatePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
