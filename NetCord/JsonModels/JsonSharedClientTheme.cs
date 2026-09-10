using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonSharedClientTheme
{
    [JsonPropertyName("colors")]
    public Color[] Colors { get; set; }

    [JsonPropertyName("gradient_angle")]
    public int GradientAngle { get; set; }

    [JsonPropertyName("base_mix")]
    public int BaseMix { get; set; }

    [JsonPropertyName("base_theme")]
    public SharedClientThemeBase? BaseTheme { get; set; }
}
