using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="customId">ID for the select (max 100 characters).</param>
[GenerateMethodsForProperties]
public partial class ChannelSelectProperties(string customId) : EntitySelectProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.ChannelSelect;

    /// <summary>
    /// Default values for auto-populated select components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    /// <summary>
    /// List of channel types to include in the select.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_types")]
    public IEnumerable<ChannelType>? ChannelTypes { get; set; }

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.ChannelSelectProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.ChannelSelectProperties);
    }

    public class DefaultValuesConverter : SelectPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("channel");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }
}
