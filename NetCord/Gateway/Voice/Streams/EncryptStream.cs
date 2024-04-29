﻿using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;

using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

internal class EncryptStream(Stream next, IVoiceEncryption encryption, VoiceClient client) : RewritingStream(next)
{
    private readonly int _expansion = 12 + encryption.Expansion;
    private readonly bool _extensionEncryption = encryption.ExtensionEncryption;
    private ushort _sequenceNumber = (ushort)RandomNumberGenerator.GetInt32(ushort.MaxValue);
    private uint _timestamp = (uint)RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int datagramLength = buffer.Length + _expansion;
        using var owner = MemoryPool<byte>.Shared.Rent(datagramLength);
        var datagram = owner.Memory[..datagramLength];
        WriteDatagram(buffer.Span, datagram.Span);
        await _next.WriteAsync(datagram, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int datagramLength = buffer.Length + _expansion;
        using var owner = MemoryPool<byte>.Shared.Rent(datagramLength);
        var datagram = owner.Memory.Span[..datagramLength];
        WriteDatagram(buffer, datagram);
        _next.Write(datagram);
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> bytes = new([0xF8, 0xFF, 0xFE]);
        for (int i = 0; i < 5; i++)
            await WriteAsync(bytes, cancellationToken).ConfigureAwait(false);

        await base.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Flush()
    {
        ReadOnlySpan<byte> bytes = [0xF8, 0xFF, 0xFE];
        for (int i = 0; i < 5; i++)
            Write(bytes);

        base.Flush();
    }

    private void WriteDatagram(ReadOnlySpan<byte> buffer, Span<byte> datagram)
    {
        WriteRtpHeader(datagram);
        encryption.Encrypt(buffer, new(datagram, _extensionEncryption));
    }

    private void WriteRtpHeader(Span<byte> datagram)
    {
        datagram[0] = 0b10000000;
        datagram[1] = 0b01111000;
        BinaryPrimitives.WriteUInt16BigEndian(datagram[2..], ++_sequenceNumber);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[4..], _timestamp += Opus.SamplesPerChannel);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[8..], client.Cache.Ssrc);
    }
}
