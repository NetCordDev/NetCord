using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class RoleColorsProperties(Color primaryColor)
{
    [JsonPropertyName("primary_color")]
    public Color PrimaryColor { get; set; } = primaryColor;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("secondary_color")]
    public Color? SecondaryColor { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tertiary_color")]
    public Color? TertiaryColor { get; set; }
}
