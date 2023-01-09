using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ChannelMenuProperties : MenuProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Id for the menu (max 100 characters).</param>
    public ChannelMenuProperties(string customId) : base(customId, ComponentType.ChannelMenu)
    {
    }

    /// <summary>
    /// List of channel types to include in the menu.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_types")]
    public IEnumerable<ChannelType>? ChannelTypes { get; set; }

    [JsonSerializable(typeof(ChannelMenuProperties))]
    public partial class ChannelMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelMenuPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
