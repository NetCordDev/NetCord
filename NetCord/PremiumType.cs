namespace NetCord;

/// <summary>
/// Specifies a Nitro subscription's type.
/// </summary>
public enum PremiumType
{
    /// <summary>
    /// No current Nitro subscription.
    /// </summary>
    None = 0,

    /// <summary>
    /// A Nitro Classic subscription.
    /// </summary>
    NitroClassic = 1,

    /// <summary>
    /// A standard Nitro subscription.
    /// </summary>
    Nitro = 2,

    /// <summary>
    /// A Nitro Basic subscription.
    /// </summary>
    NitroBasic = 3,
}
