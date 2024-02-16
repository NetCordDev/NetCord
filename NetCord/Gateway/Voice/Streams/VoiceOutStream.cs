using NetCord.Gateway.Voice.UdpSockets;

namespace NetCord.Gateway.Voice;

internal class VoiceOutStream(IUdpSocket udpSocket) : VoiceStream
{
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => udpSocket.SendAsync(buffer, cancellationToken);

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        udpSocket.Send(buffer);
    }
}
