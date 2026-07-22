using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a shared custom client theme.
/// </summary>
public class SharedClientTheme(JsonSharedClientTheme jsonModel) : IJsonModel<JsonSharedClientTheme>
{
    public JsonSharedClientTheme JsonModel => jsonModel;

    /// <summary>
    /// A list of the colors within the theme's gradient (maximum of 5).
    /// </summary>
    public IReadOnlyList<Color> Colors => jsonModel.Colors;

    /// <summary>
    /// The direction of the theme's gradient (maximum of 360).
    /// </summary>
    public int GradientAngle => jsonModel.GradientAngle;

    /// <summary>
    /// The intensity of the theme's gradient colors (maximum of 100).
    /// </summary>
    public int BaseMix => jsonModel.BaseMix;

    /// <summary>
    /// The theme the client theme is based on.
    /// </summary>
    public BaseTheme BaseTheme => jsonModel.BaseTheme.GetValueOrDefault();
}
