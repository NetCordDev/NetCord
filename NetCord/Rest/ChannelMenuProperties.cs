using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

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
    /// Default values for auto-populated select menu components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    /// <summary>
    /// List of channel types to include in the menu.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_types")]
    public IEnumerable<ChannelType>? ChannelTypes { get; set; }

    public class DefaultValuesConverter : MenuPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("channel");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }

    [JsonSerializable(typeof(ChannelMenuProperties))]
    public partial class ChannelMenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static ChannelMenuPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
