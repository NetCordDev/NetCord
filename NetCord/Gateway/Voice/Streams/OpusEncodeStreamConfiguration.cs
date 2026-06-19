namespace NetCord.Gateway.Voice;

public class OpusEncodeStreamConfiguration
{
    /// <summary>
    /// The duration of each Opus frame, in milliseconds. Defaults to <see cref="Opus.DefaultFrameDuration"/>. Allowed values are 2.5, 5, 10, 20, 40, 60, 80, 100 and 120 (the last one is only allowed for <see cref="VoiceChannels.Mono"/>).
    /// </summary>
    public float? FrameDuration { get; set; }

    /// <summary>
    /// Whether to segment the written data into Opus frames. You can set this to <see langword="false"/> if you are sure to write exactly one Opus frame at a time. Defaults to <see langword="true"/>.
    /// </summary>
    public bool? Segment { get; set; }
}
