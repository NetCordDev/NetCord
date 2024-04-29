using System.Buffers;
using System.Buffers.Binary;

using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

internal class DecryptStream(Stream next, IVoiceEncryption encryption) : RewritingStream(next)
{
    private readonly int _expansion = 12 + encryption.Expansion;
    private readonly bool _extensionEncryption = encryption.ExtensionEncryption;
    private bool _used;
    private ushort _sequenceNumber;

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        RtpPacketStorage packetStorage = new(buffer, _extensionEncryption);

        if (packetStorage.Packet.PayloadType is not 0x78)
            return;

        var sequenceNumber = packetStorage.Packet.SequenceNumber;

        if (_used)
        {
            var diff = (ushort)(sequenceNumber - _sequenceNumber);
            switch (diff)
            {
                case 1:
                    {
                        _sequenceNumber = sequenceNumber;
                    }
                    break;
                case 0 or > ushort.MaxValue / 2:
                    return;
                default:
                    {
                        _sequenceNumber = sequenceNumber;
                        int max = diff - 1;
                        var tasks = new ValueTask[max];
                        int i = 0;
                        do
#pragma warning disable CA2012 // Use ValueTasks correctly
                            tasks[i] = _next.WriteAsync(null, cancellationToken);
#pragma warning restore CA2012 // Use ValueTasks correctly
                        while (++i < max);

                        var actual = ActualWriteAsync();

                        i = 0;
                        do
                            await tasks[i].ConfigureAwait(false);
                        while (++i < max);

                        await actual.ConfigureAwait(false);
                    }
                    return;
            }
        }
        else
        {
            _used = true;
            _sequenceNumber = sequenceNumber;
        }

        await ActualWriteAsync().ConfigureAwait(false);

        async ValueTask ActualWriteAsync()
        {
            using (Decrypt(out var plaintext))
                await _next.WriteAsync(plaintext, cancellationToken).ConfigureAwait(false);

            IMemoryOwner<byte> Decrypt(out Memory<byte> plaintext)
            {
                var packet = packetStorage.Packet;

                var extension = packet.Extension;

                int plaintextLength = extension && !_extensionEncryption ? buffer.Length - _expansion - 4 : buffer.Length - _expansion;

                var owner = MemoryPool<byte>.Shared.Rent(plaintextLength);
                plaintext = owner.Memory[..plaintextLength];
                var plaintextSpan = plaintext.Span;

                encryption.Decrypt(packet, plaintextSpan);

                if (extension)
                {
                    int extensionLength = _extensionEncryption ? BinaryPrimitives.ReadUInt16BigEndian(plaintextSpan[2..]) + 1 : BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[(packet.HeaderLength + 2)..]);
                    plaintext = plaintext[(4 * extensionLength)..];
                }

                return owner;
            }
        }
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        RtpPacket packet = new(buffer, _extensionEncryption);

        if (packet.PayloadType is not 0x78)
            return;

        var sequenceNumber = packet.SequenceNumber;

        if (_used)
        {
            var diff = (ushort)(sequenceNumber - _sequenceNumber);
            switch (diff)
            {
                case 1:
                    {
                        _sequenceNumber = sequenceNumber;
                    }
                    break;
                case 0 or > ushort.MaxValue / 2:
                    return;
                default:
                    {
                        _sequenceNumber = sequenceNumber;
                        int i = 1;
                        do
                            _next.Write(null);
                        while (++i < diff);
                    }
                    break;
            }
        }
        else
        {
            _used = true;
            _sequenceNumber = sequenceNumber;
        }

        var extension = packet.Extension;

        int plaintextLength = extension && !_extensionEncryption ? buffer.Length - _expansion - 4 : buffer.Length - _expansion;

        var owner = MemoryPool<byte>.Shared.Rent(plaintextLength);
        var plaintext = owner.Memory.Span[..plaintextLength];

        encryption.Decrypt(packet, plaintext);

        if (extension)
        {
            int extensionLength = _extensionEncryption ? BinaryPrimitives.ReadUInt16BigEndian(plaintext[2..]) + 1 : BinaryPrimitives.ReadUInt16BigEndian(packet.Datagram[(packet.HeaderLength + 2)..]);
            plaintext = plaintext[(4 * extensionLength)..];
        }

        _next.Write(plaintext);
    }
}
