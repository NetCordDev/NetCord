using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

/// <summary>
/// Voice encryption provider.
/// </summary>
public interface IVoiceEncryptionProvider
{
    /// <summary>
    /// Selects and returns an appropriate voice encryption implementation based on the provided encryption modes.
    /// </summary>
    /// <param name="modes">A list of encryption modes supported by the Discord backend.</param>
    /// <returns>An implementation of <see cref="IVoiceEncryption"/> that matches one of the specified modes.</returns>
    public IVoiceEncryption GetEncryption(IReadOnlyList<string> modes);
}
