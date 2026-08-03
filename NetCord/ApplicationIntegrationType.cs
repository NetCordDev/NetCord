namespace NetCord;

/// <summary>
/// Represents an app integration's install type.
/// </summary>
public enum ApplicationIntegrationType
{
    /// <summary>
    /// App is available for guild installs.
    /// </summary>
    GuildInstall = 0,

    /// <summary>
    /// App is available for user installs.
    /// </summary>
    UserInstall = 1,
}
