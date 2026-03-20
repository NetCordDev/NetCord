using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class RoleMenuProperties(string customId) : EntityMenuProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.RoleMenu;

    /// <summary>
    /// Default values for auto-populated select menu components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.RoleMenuProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.RoleMenuProperties);
    }

    public class DefaultValuesConverter : MenuPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("role");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }
}
