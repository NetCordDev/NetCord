namespace NetCord.Gateway.Voice;

public class VoiceStreamConfiguration
{
    /// <summary>
    /// The duration of the Opus frames that will be written to the stream, in milliseconds. Defaults to <see cref="Opus.DefaultFrameDuration"/>.
    /// </summary>
    public float? FrameDuration { get; set; }

    /// <summary>
    /// Whether to normalize the voice sending speed. Defaults to <see langword="true"/>.
    /// </summary>
    public bool? NormalizeSpeed { get; set; }
}
