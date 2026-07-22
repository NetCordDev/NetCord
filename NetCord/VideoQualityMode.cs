namespace NetCord;

/// <summary>
/// Determines Discord's behaviour towards video quality optimization.
/// </summary>
public enum VideoQualityMode
{
    /// <summary>
    /// No video quality mode specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Allows discord to adjust the quality for optimal performance.
    /// </summary>
    Auto = 1,

    /// <summary>
    /// Locks all video to 720p.
    /// </summary>
    Full = 2,
}
