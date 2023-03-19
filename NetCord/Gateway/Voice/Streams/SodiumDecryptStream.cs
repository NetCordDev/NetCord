using System.Buffers;
using System.Buffers.Binary;

namespace NetCord.Gateway.Voice;

internal class SodiumDecryptStream : RewritingStream
{
    private bool _used;
    private ushort _sequenceNumber;
    private readonly VoiceClient _client;

    public SodiumDecryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
    }

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
                            tasks[i] = _next.WriteAsync(null, cancellationToken);
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
            int bufferLen = buffer.Length - 12;
            int resultLen = bufferLen - Libsodium.MacBytes;
            using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
            await _next.WriteAsync(GetDecrypted(), cancellationToken).ConfigureAwait(false);

            Memory<byte> GetDecrypted()
            {
                var result = owner.Memory;
                var resultSpan = result.Span;
                var bufferSpan = buffer.Span;
                Decrypt(resultSpan, bufferSpan, bufferLen);

                if ((bufferSpan[0] & 0b10000) != 0)
                {
                    var length = BinaryPrimitives.ReadUInt16BigEndian(resultSpan[2..]);
                    result = result[(4 * (length + 1))..resultLen];
                }
                else
                    result = result[..resultLen];

                return result;
            }
        }
    }

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
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

        int bufferLen = buffer.Length - 12;
        int resultLen = bufferLen - Libsodium.MacBytes;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory.Span;
        Decrypt(result, buffer, bufferLen);

        if ((buffer[0] & 0b10000) != 0)
        {
            var length = BinaryPrimitives.ReadUInt16BigEndian(result[2..]);
            result = result[(4 * (length + 1))..resultLen];
        }
        else
            result = result[..resultLen];

        _next.Write(result);
    }

    private unsafe void Decrypt(Span<byte> result, ReadOnlySpan<byte> buffer, int bufferLen)
    {
        var noncePtr = stackalloc byte[Libsodium.NonceBytes];
        Span<byte> nonce = new(noncePtr, Libsodium.NonceBytes);
        buffer[..12].CopyTo(nonce);

        int code;
        fixed (byte* resultPtr = result, bufferPtr = buffer)
            code = Libsodium.CryptoSecretboxOpenEasy(resultPtr, bufferPtr + 12, (ulong)bufferLen, noncePtr, _client._secretKey!);

        if (code != 0)
            throw new LibsodiumException();
    }
}
