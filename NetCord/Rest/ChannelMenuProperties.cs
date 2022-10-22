using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ChannelMenuProperties : MenuProperties
{
    public ChannelMenuProperties(string customId) : base(customId, ComponentType.ChannelMenu)
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_types")]
    public IEnumerable<ChannelType>? ChannelTypes { get; set; }

    [JsonSerializable(typeof(ChannelMenuProperties))]
    public partial class ChannelMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelMenuPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
