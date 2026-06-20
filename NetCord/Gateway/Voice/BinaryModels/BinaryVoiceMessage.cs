using System.Buffers.Binary;

namespace NetCord.Gateway.Voice.BinaryModels;

internal readonly ref struct BinaryVoiceMessage(ReadOnlySpan<byte> message)
{
    private readonly ReadOnlySpan<byte> _message = message;

    public ushort SequenceNumber => BinaryPrimitives.ReadUInt16BigEndian(_message);

    public VoiceOpcode Opcode => (VoiceOpcode)_message[2];

    public ReadOnlySpan<byte> Data => _message[3..];
}
