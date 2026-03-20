namespace NetCord.Gateway.Voice;

public class VoiceReceiveHandler : IVoiceReceiveHandler
{
    private readonly Dictionary<uint, ushort> _lastSequenceNumbers = [];

    public bool RequiresExternalSocketAddress => true;

    public VoicePacketHandlingResult HandlePacket(VoiceClient client, RtpPacket packet)
    {
        ushort framesMissed;
        bool handle;

        if (packet.PayloadType is not 0x78)
            goto Fail;

        var lastSequenceNumbers = _lastSequenceNumbers;
        var sequenceNumber = packet.SequenceNumber;

        if (lastSequenceNumbers.TryGetValue(packet.Ssrc, out var lastSequenceNumber))
        {
            var sequenceNumberDiff = (short)(sequenceNumber - lastSequenceNumber);
            if (sequenceNumberDiff <= 0)
                goto Fail;

            framesMissed = (ushort)(sequenceNumberDiff - 1);
        }
        else
            framesMissed = 0;

        handle = true;
        lastSequenceNumbers[packet.Ssrc] = sequenceNumber;

        Ret:
        return new(framesMissed, handle);

        Fail:
        framesMissed = 0;
        handle = false;
        goto Ret;
    }
}
