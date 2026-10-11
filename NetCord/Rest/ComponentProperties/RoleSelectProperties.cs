using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class RoleSelectProperties(string customId) : EntitySelectProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.RoleSelect;

    /// <summary>
    /// Default values for auto-populated select components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.RoleSelectProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.RoleSelectProperties);
    }

    public class DefaultValuesConverter : SelectPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("role");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }
}
