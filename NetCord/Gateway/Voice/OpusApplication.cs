namespace NetCord.Gateway.Voice;

/// <summary>
/// Opus coding mode.
/// </summary>
public enum OpusApplication
{
    /// <summary>
    /// <see cref="Voip"/> gives best quality at a given bitrate for voice signals. It enhances the input signal by high-pass filtering and emphasizing formants and harmonics. Optionally it includes in-band forward error correction to protect against packet loss. Use this mode for typical VoIP applications. Because of the enhancement, even at high bitrates the output may sound different from the input.
    /// </summary>
    /// <remarks>
    /// Best for most VoIP/videoconference applications where listening quality and intelligibility matter most.
    /// </remarks>
    Voip = 2048,

    /// <summary>
    /// <see cref="Audio"/> gives best quality at a given bitrate for most non-voice signals like music. Use this mode for music and mixed (music/voice) content, broadcast, and applications requiring less than 15 ms of coding delay.
    /// </summary>
    /// <remarks>
    /// Best for broadcast/high-fidelity application where the decoded audio should be as close as possible to the input.
    /// </remarks>
    Audio = 2049,

    /// <summary>
    /// <see cref="RestrictedLowdelay"/> configures low-delay mode that disables the speech-optimized mode in exchange for slightly reduced delay. This mode can only be set on an newly initialized or freshly reset encoder because it changes the codec delay.
    /// </summary>
    /// <remarks>
    /// Only use when lowest-achievable latency is what matters most. Voice-optimized modes cannot be used.
    /// </remarks>
    RestrictedLowdelay = 2051,
}
