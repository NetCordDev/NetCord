namespace NetCord.Gateway.Voice;

/// <summary>
/// Format of PCM.
/// </summary>
public enum PcmFormat : byte
{
    /// <summary>
    /// 16-bit signed integer format.
    /// </summary>
    Short = sizeof(short),

    /// <summary>
    /// 32-bit floating-point format.
    /// </summary>
    Float = sizeof(float),
}
