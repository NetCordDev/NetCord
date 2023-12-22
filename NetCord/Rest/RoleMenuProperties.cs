using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public partial class RoleMenuProperties : MenuProperties
{
    public RoleMenuProperties(string customId) : base(customId, ComponentType.RoleMenu)
    {
    }

    /// <summary>
    /// Default values for auto-populated select menu components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    public class DefaultValuesConverter : MenuPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("role");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }
}
