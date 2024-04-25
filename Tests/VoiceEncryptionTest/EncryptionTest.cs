using System.Buffers.Binary;

using NetCord.Gateway.Voice.Encryption;

namespace VoiceEncryptionTest;

[TestClass]
public class EncryptionTest
{
    [TestMethod]
    public void XSalsa20Poly1305EncryptionTest()
    {
        using XSalsa20Poly1305Encryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void XSalsa20Poly1305LiteEncryptionTest()
    {
        using XSalsa20Poly1305LiteEncryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void XSalsa20Poly1305SuffixEncryptionTest()
    {
        using XSalsa20Poly1305SuffixEncryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void Aes256GcmEncryptionTest()
    {
        using Aes256GcmEncryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void XSalsa20Poly1305LiteRtpSizeEncryptionTest()
    {
        using XSalsa20Poly1305LiteRtpSizeEncryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void Aes256GcmRtpSizeEncryptionTest()
    {
        using Aes256GcmRtpSizeEncryption encryption = new();
        TestEncryption(encryption);
    }

    [TestMethod]
    public void XChaCha20Poly1305RtpSizeEncryptionTest()
    {
        using XChaCha20Poly1305RtpSizeEncryption encryption = new();
        TestEncryption(encryption);
    }

    private static void TestEncryption(IVoiceEncryption encryption)
    {
        TestEncryption(encryption, HeaderExtensionInfo.None);

        for (ushort length = 0; length < 10; length++)
        {
            TestEncryption(encryption, HeaderExtensionInfo.OneByte(length));
            TestEncryption(encryption, HeaderExtensionInfo.TwoByte(length));
        }
    }

    private static void TestEncryption(IVoiceEncryption encryption, HeaderExtensionInfo headerExtension)
    {
        var key = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();
        encryption.SetKey(key);

        int dataLength = 324;

        var plaintext = GetData(dataLength, encryption.ExtensionEncryption, headerExtension);

        var datagram = CreateDatagram(dataLength, encryption.Expansion, encryption.ExtensionEncryption, headerExtension);

        encryption.Encrypt(plaintext, new(datagram, encryption.ExtensionEncryption));

        var plaintext2 = new byte[plaintext.Length];

        encryption.Decrypt(new(datagram, encryption.ExtensionEncryption), plaintext2);

        CollectionAssert.AreEqual(plaintext, plaintext2);
    }

    private static byte[] CreateDatagram(int dataLength, int expansion, bool extensionEncryption, HeaderExtensionInfo headerExtension)
    {
        int length = 12 + expansion + dataLength;

        if (headerExtension.Enabled)
            length += 4 + (4 * headerExtension.Length);

        var result = new byte[length];

        var datagram = result.AsSpan();

        datagram[0] = headerExtension.Enabled ? (byte)0x90 : (byte)0x80;
        datagram[1] = 0x78;

        if (!extensionEncryption && headerExtension.Enabled)
            headerExtension.Write(datagram[12..]);

        return result;
    }

    private static byte[] GetData(int length, bool extensionEncryption, HeaderExtensionInfo headerExtension)
    {
        int extensionLength = 0;

        if (headerExtension.Enabled)
        {
            extensionLength += 4 * headerExtension.Length;

            if (extensionEncryption)
                extensionLength += 4;
        }

        length += extensionLength;

        var result = new byte[length];

        var data = result.AsSpan();

        if (extensionEncryption && headerExtension.Enabled)
            headerExtension.Write(data);

        for (int i = extensionLength; i < length; i++)
            data[i] = (byte)i;

        return result;
    }

    private readonly record struct HeaderExtensionInfo(ushort DefinedByProfile, ushort Length)
    {
        public bool Enabled { get; } = true;

        public static HeaderExtensionInfo None => default;

        public static HeaderExtensionInfo OneByte(ushort length) => new(0xBEDE, length);

        public static HeaderExtensionInfo TwoByte(ushort length) => new(0x100, length);

        public readonly void Write(Span<byte> span)
        {
            if (!Enabled)
                throw new InvalidOperationException("Header extension is not enabled.");

            BinaryPrimitives.WriteUInt16BigEndian(span, DefinedByProfile);
            BinaryPrimitives.WriteUInt16BigEndian(span[2..], Length);
        }
    }
}
