namespace NetCord;

/// <summary>
/// Specifies a <see cref="SharedClientTheme"/>'s base theme.
/// </summary>
public enum BaseTheme : byte
{
    /// <summary>
    /// No base theme specified, equivalent to <see cref="Dark"/>.
    /// </summary>
    Unset = 0,

    /// <summary>
    /// Equivalent to Discord's 'Ash' theme.
    /// </summary>
    Dark = 1,

    /// <summary>
    /// Equivalent to Discord's 'Light' theme.
    /// </summary>
    Light = 2,

    /// <summary>
    /// Equivalent to Discord's 'Dark' theme.
    /// </summary>
    Darker = 3,

    /// <summary>
    /// Equivalent to Discord's 'Onyx' theme.
    /// </summary>
    Midnight = 4
}
