namespace NetCord;

/// <summary>
/// Specifies the API version used in request paths.
/// </summary>
public enum ApiVersion
{
    [Obsolete("Discontinued on the 16th of October, 2017.")]
    V3 = 3,

    [Obsolete("Discontinued on the 16th of October, 2017.")]
    V4 = 4,

    [Obsolete("Discontinued on the 16th of October, 2017.")]
    V5 = 5,

    [Obsolete("Deprecated")]
    V6 = 6,

    [Obsolete("Deprecated")]
    V7 = 7,

    [Obsolete("Deprecated")]
    V8 = 8,

    V9 = 9,

    V10 = 10,
}
