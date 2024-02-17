using System.Buffers;
using System.Buffers.Binary;

using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

internal class DecryptStream(Stream next, IVoiceEncryption encryption) : RewritingStream(next)
{
    private readonly int _expansion = encryption.Expansion + 12;
    private bool _used;
    private ushort _sequenceNumber;

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var sequenceNumber = BinaryPrimitives.ReadUInt16BigEndian(buffer.Span[2..]);
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
            int plaintextLen = buffer.Length - _expansion;
            using var owner = MemoryPool<byte>.Shared.Rent(plaintextLen);
            var plaintext = owner.Memory[..plaintextLen];
            await _next.WriteAsync(EncryptAndGetWithoutHeaderExtension(buffer, plaintext), cancellationToken).ConfigureAwait(false);

            Memory<byte> EncryptAndGetWithoutHeaderExtension(ReadOnlyMemory<byte> buffer, Memory<byte> plaintext)
            {
                var bufferSpan = buffer.Span;
                var plaintextSpan = plaintext.Span;

                encryption.Decrypt(bufferSpan, plaintextSpan);
                return (bufferSpan[0] & 0b10000) == 0 ? plaintext
                                                      : plaintext[(4 * (BinaryPrimitives.ReadUInt16BigEndian(plaintextSpan[2..]) + 1))..];
            }
        }
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        var sequenceNumber = BinaryPrimitives.ReadUInt16BigEndian(buffer[2..]);
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

        int plaintextLen = buffer.Length - _expansion;
        using var owner = MemoryPool<byte>.Shared.Rent(plaintextLen);
        var plaintext = owner.Memory.Span[..plaintextLen];
        encryption.Decrypt(buffer, plaintext);
        _next.Write((buffer[0] & 0b10000) == 0 ? plaintext
                                               : plaintext[(4 * (BinaryPrimitives.ReadUInt16BigEndian(plaintext[2..]) + 1))..]);
    }
}
