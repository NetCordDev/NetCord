using System.Diagnostics.CodeAnalysis;

namespace NetCord.Gateway.Voice;

public readonly ref struct VoiceReceiveEventArgs(byte[]? buffer, int frameIndex, int frameLength, uint ssrc)
{
    internal readonly byte[]? _buffer = buffer;

    public ReadOnlySpan<byte> Frame => _buffer.AsSpan(frameIndex, frameLength);

    public uint Ssrc => ssrc;

    //internal VoiceReceiveEventArgs(RtpPacketStorage packetStorage, IVoiceEncryption encryption)
    //{
    //    _packetStorage = packetStorage;
    //    _encryption = encryption;
    //}

    //private readonly RtpPacketStorage _packetStorage;
    //private readonly IVoiceEncryption _encryption;

    //public uint Ssrc => _packetStorage.Packet.Ssrc;

    //public int PayloadSize
    //{
    //    get
    //    {
    //        return _packetStorage.Packet.PayloadLength - _encryption.Expansion;
    //    }
    //}

    //public ushort SequenceNumber => _packetStorage.Packet.SequenceNumber;

    //public uint Timestamp => _packetStorage.Packet.Timestamp;

    //public VoiceDecryptionResult Decrypt(Span<byte> buffer)
    //{
    //    var extensionLength = DecryptCore(buffer);

    //    return new(extensionLength);
    //}

    //public VoicePacketHandlingResult GetContext(ushort lastSequenceNumber)
    //{
    //    var sequenceNumberDiff = (short)(SequenceNumber - lastSequenceNumber);
    //    return sequenceNumberDiff <= 0 ? new(0, false) : new((ushort)(sequenceNumberDiff - 1), true);
    //}

    //public ContinuousVoiceDecryptionResult Decrypt(Span<byte> buffer, ushort? lastSequenceNumber = null)
    //{
    //    int frameIndex;
    //    ushort framesMissed;
    //    ContinuousVoiceDecryptionStatus status;

    //    if (lastSequenceNumber.HasValue)
    //    {
    //        var sequenceNumberDiff = GetSequenceNumberDiff(lastSequenceNumber.GetValueOrDefault());

    //        if (sequenceNumberDiff <= 0)
    //        {
    //            frameIndex = 0;
    //            framesMissed = 0;
    //            status = ContinuousVoiceDecryptionStatus.OutOfOrder;
    //            goto Ret;
    //        }

    //        framesMissed = (ushort)(sequenceNumberDiff - 1);
    //    }
    //    else
    //        framesMissed = 0;

    //    frameIndex = DecryptCore(buffer);
    //    status = ContinuousVoiceDecryptionStatus.Ok;
    //    Ret:
    //    return new(frameIndex, framesMissed, status);
    //}

    //private int DecryptCore(Span<byte> buffer)
    //{
    //    int frameSize = PayloadSize;
    //    if (buffer.Length < frameSize)
    //        ThrowBufferTooSmall(frameSize, buffer.Length);

    //    var packet = _packetStorage.Packet;

    //    _encryption.Decrypt(packet, buffer[..frameSize]);

    //    int extensionLength;

    //    if (packet.Extension)
    //    {
    //        extensionLength = 4 * (_encryption.ExtensionEncryption
    //            ? BinaryPrimitives.ReadUInt16BigEndian(buffer[2..]) + 1
    //            : BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[(packet.HeaderLength + 2)..]));
    //    }
    //    else
    //        extensionLength = 0;

    //    return extensionLength;
    //}

    [DoesNotReturn]
    private static void ThrowBufferTooSmall(int requiredSize, int actualSize)
    {
        throw new ArgumentException($"Buffer must be at least {requiredSize} bytes long, but was {actualSize} bytes long.", "buffer");
    }
}
