namespace NetCord.Gateway.Voice;

internal struct VoiceEncryptionInfo
{
    private const int ExpansionMask = 0x7FFFFFFF;

    private const int ExtensionEncryptionMask = 1 << 31;

    private readonly int _data;

    public VoiceEncryptionInfo(int expansion, bool extensionEncryption)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(expansion, nameof(expansion));

        _data = expansion | (extensionEncryption ? ExtensionEncryptionMask : 0);
    }

    public readonly int Expansion => _data & ExpansionMask;

    public readonly bool ExtensionEncryption => (_data & ExtensionEncryptionMask) is not 0;
}
