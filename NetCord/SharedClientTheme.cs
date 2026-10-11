using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a shared custom client theme.
/// </summary>
public class SharedClientTheme(JsonSharedClientTheme jsonModel)
{
    /// <summary>
    /// A list of the colors within the theme's gradient (maximum of 5).
    /// </summary>
    public IReadOnlyList<Color> Colors { get; } = jsonModel.Colors;

    /// <summary>
    /// The direction of the theme's gradient (maximum of 360).
    /// </summary>
    public int GradientAngle { get; } = jsonModel.GradientAngle;

    /// <summary>
    /// The intensity of the theme's gradient colors (maximum of 100).
    /// </summary>
    public int BaseMix { get; } = jsonModel.BaseMix;

    /// <summary>
    /// The theme the client theme is based on.
    /// </summary>
    public SharedClientThemeBase? BaseTheme { get; } = jsonModel.BaseTheme;
}
