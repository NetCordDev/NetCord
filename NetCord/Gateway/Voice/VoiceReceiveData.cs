// using System.Runtime.InteropServices;

// namespace NetCord.Gateway.Voice;

// [StructLayout(LayoutKind.Auto)]
// public readonly ref struct VoiceReceiveData
// {
//     private VoiceReceiveData(uint ssrc, ushort sequenceNumber, RtpPacket packet, Flags flags)
//     {
//         Ssrc = ssrc;
//         SequenceNumber = sequenceNumber;
//         Packet = packet;
//         _flags = flags;
//     }

//     public static VoiceReceiveData FromPacket(RtpPacket packet, bool canCorrectLoss = false)
//     {
//         return new(packet.Ssrc, packet.SequenceNumber, packet, canCorrectLoss ? Flags.HasPacket | Flags.CanCorrectLoss : Flags.HasPacket);
//     }

//     // public static VoiceReceiveData FromNextPacket(RtpPacket packet)
//     // {
//     //     return new(packet.Ssrc, (ushort)(packet.SequenceNumber - 1), packet, Flags.HasPacket | Flags.Lost);
//     // }

//     public static VoiceReceiveData Lost(uint ssrc, ushort sequenceNumber)
//     {
//         return new(ssrc, sequenceNumber, default, default);
//     }

//     public RtpPacket Packet { get; }

//     public uint Ssrc { get; }

//     public ushort SequenceNumber { get; }

//     public bool HasPacket => _flags.HasFlag(Flags.HasPacket);

//     public bool CanCorrectLoss => _flags.HasFlag(Flags.CanCorrectLoss);

//     // public bool IsLost => _flags.HasFlag(Flags.Lost);

//     private readonly Flags _flags;

//     [Flags]
//     private enum Flags : byte
//     {
//         HasPacket = 1 << 0,
//         CanCorrectLoss = 1 << 1,
//         // Lost = 1 << 1
//     }
// }
