namespace NetCord.Gateway.Voice.Encryption;

/// <summary>
/// Voice encryption.
/// </summary>
public interface IVoiceEncryption
{
    /// <summary>
    /// The name of the encryption algorithm.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The expansion of the encryption algorithm.
    /// </summary>
    public int Expansion { get; }

    /// <summary>
    /// Decrypts a datagram using the encryption algorithm.
    /// </summary>
    /// <param name="datagram">The datagram to decrypt.</param>
    /// <param name="plaintext">The resulting plaintext.</param>
    public void Decrypt(ReadOnlySpan<byte> datagram, Span<byte> plaintext);

    /// <summary>
    /// Encrypts plaintext using the encryption algorithm.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="datagram">The resulting datagram.</param>
    public void Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> datagram);

    /// <summary>
    /// Sets the key for the encryption algorithm.
    /// </summary>
    /// <param name="key">The key to set.</param>
    public void SetKey(byte[] key);
}
