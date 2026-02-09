namespace NetCord.Gateway.Voice;

public class VoiceStreamConfiguration
{
    /// <summary>
    /// The duration of the Opus frames that will be written to the stream, in milliseconds. Defaults to <see cref="Opus.DefaultFrameDuration"/>. Allowed values are 2.5, 5, 10, 20, 40, 60 and 120 (the last one is only allowed for <see cref="VoiceChannels.Mono"/>).
    /// </summary>
    public float? FrameDuration { get; set; }

    /// <summary>
    /// Whether to normalize the voice sending speed. Defaults to <see langword="true"/>.
    /// </summary>
    public bool? NormalizeSpeed { get; set; }
}
