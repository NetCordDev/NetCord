namespace NetCord.Gateway.Voice;

public class OpusEncodeStreamConfiguration
{
    /// <summary>
    /// The duration of each Opus frame, in milliseconds. Defaults to <see cref="Opus.DefaultFrameDuration"/>.
    /// </summary>
    public float? FrameDuration { get; set; }

    /// <summary>
    /// Whether to segment the written data into Opus frames. You can set this to <see langword="false"/> if you are sure to write exactly one Opus frame at a time. Defaults to <see langword="true"/>.
    /// </summary>
    public bool? Segment { get; set; }
}
