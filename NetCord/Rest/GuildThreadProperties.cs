using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildThreadProperties : GuildThreadFromMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public ChannelType? ChannelType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }

    public GuildThreadProperties(string name) : base(name)
    {
    }

    [JsonSerializable(typeof(GuildThreadProperties))]
    public partial class GuildThreadPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildThreadPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
