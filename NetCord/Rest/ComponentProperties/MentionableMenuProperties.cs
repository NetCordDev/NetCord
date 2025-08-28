using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MentionableMenuProperties(string customId) : EntityMenuProperties(customId)
{
    public override ComponentType ComponentType => ComponentType.MentionableMenu;

    /// <summary>
    /// Default values for auto-populated select menu components.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_values")]
    public IEnumerable<MentionableValueProperties>? DefaultValues { get; set; }

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.MentionableMenuProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.MentionableMenuProperties);
    }
}
