using System.Buffers.Binary;

namespace NetCord.Gateway.Voice.BinaryModels;

internal readonly ref struct BinaryVoiceMessage(ReadOnlySpan<byte> payload)
{
    private readonly ReadOnlySpan<byte> _payload = payload;

    public ushort SequenceNumber => BinaryPrimitives.ReadUInt16BigEndian(_payload);

    public VoiceOpcode Opcode => (VoiceOpcode)_payload[2];

    public ReadOnlySpan<byte> Data => _payload[3..];
}
