using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class ActionButtonProperties : ButtonProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    public ActionButtonProperties(string customId, ButtonStyle style) : base(style)
    {
        CustomId = customId;
    }
}