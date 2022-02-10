using System.Text.Json.Serialization;

namespace NetCord;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class ActionButtonProperties : ButtonProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    public ActionButtonProperties(string label, string customId, ButtonStyle style) : base(label, style)
    {
        CustomId = customId;
    }
}