using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

public partial class UserMenuProperties(string customId) : EntityMenuProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.UserMenu;

    /// <summary>
    /// Default values for auto-populated select menu components.
    /// </summary>
    [JsonConverter(typeof(DefaultValuesConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<ulong>? DefaultValues { get; set; }

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.UserMenuProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.UserMenuProperties);
    }

    public class DefaultValuesConverter : MenuPropertiesDefaultValuesConverter
    {
        private static readonly JsonEncodedText _typeValue = JsonEncodedText.Encode("user");

        public DefaultValuesConverter() : base(_typeValue)
        {
        }
    }
}
