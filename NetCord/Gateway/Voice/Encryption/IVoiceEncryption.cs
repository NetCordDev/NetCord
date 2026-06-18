namespace NetCord.Gateway.Voice.Encryption;

/// <summary>
/// Voice encryption.
/// </summary>
public interface IVoiceEncryption : IDisposable
{
    /// <summary>
    /// Indicates whether the encryption algorithm is supported on the current platform.
    /// </summary>
    public static abstract bool IsSupported { get; }

    /// <summary>
    /// The name of the encryption algorithm.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The expansion of the encryption algorithm.
    /// </summary>
    public int Expansion { get; }

    /// <inheritdoc cref="Decrypt"/>
    /// <returns><see langword="true"/> if decryption was successful; otherwise, <see langword="false"/>.</returns>
    public bool TryDecrypt(RtpPacket packet, Span<byte> plaintext);

    /// <inheritdoc cref="Encrypt"/>
    /// <returns><see langword="true"/> if encryption was successful; otherwise, <see langword="false"/>.</returns>
    public bool TryEncrypt(ReadOnlySpan<byte> plaintext, RtpPacketWriter packet);

    /// <summary>
    /// Decrypts a datagram using the encryption algorithm.
    /// </summary>
    /// <param name="packet">The packet to decrypt.</param>
    /// <param name="plaintext">The resulting plaintext.</param>
    public void Decrypt(RtpPacket packet, Span<byte> plaintext);

    /// <summary>
    /// Encrypts plaintext using the encryption algorithm.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="packet">The resulting packet.</param>
    public void Encrypt(ReadOnlySpan<byte> plaintext, RtpPacketWriter packet);

    /// <summary>
    /// Sets the key for the encryption algorithm.
    /// </summary>
    /// <param name="key">The key to set.</param>
    public void SetKey(byte[] key);
}
