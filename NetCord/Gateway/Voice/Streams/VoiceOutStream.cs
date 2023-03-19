using NetCord.Gateway.Voice.UdpSockets;

namespace NetCord.Gateway.Voice;

internal class VoiceOutStream : VoiceStream
{
    private readonly IUdpSocket _udpSocket;

    public VoiceOutStream(IUdpSocket udpSocket)
    {
        _udpSocket = udpSocket;
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _udpSocket.SendAsync(buffer, cancellationToken);

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _udpSocket.Send(buffer);
    }
}
