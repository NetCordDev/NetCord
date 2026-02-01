using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

internal class VoiceOutStream(VoiceClient client) : Stream
{
    private ushort _sequenceNumber = (ushort)RandomNumberGenerator.GetInt32(ushort.MaxValue);
    private uint _timestamp = (uint)RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
        ReadOnlySpan<byte> bytes = [0xF8, 0xFF, 0xFE];

        for (int i = 0; i < 5; i++)
            Write(bytes);
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> bytes = new([0xF8, 0xFF, 0xFE]);

        for (int i = 0; i < 5; i++)
            await WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(new ReadOnlySpan<byte>(buffer, offset, count));
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (client._udpState is not { Connection: var connection, Encryption: var encryption, DaveSession: var session })
        {
            ThrowConnectionNotStarted();
            return;
        }

        byte[]? daveArray = null;
        if (session.GetProtocolVersion() is not 0)
        {
            var daveEncryptor = session.GetEncryptor();
            var length = daveEncryptor.GetMaxCiphertextByteSize(Dave.MediaType.Audio, buffer.Length);
            daveArray = ArrayPool<byte>.Shared.Rent(length);

            int bytesWritten;

            while (true)
            {
                var result = daveEncryptor.Encrypt(Dave.MediaType.Audio, client.Cache.Ssrc, buffer, daveArray, out bytesWritten);

                if (result is Dave.EncryptorResultCode.Success)
                    break;

                if (result is Dave.EncryptorResultCode.MissingKeyRatchet)
                {
                    ArrayPool<byte>.Shared.Return(daveArray);
                    return;
                }

                if (result is Dave.EncryptorResultCode.TooManyAttempts)
                    continue;

                ArrayPool<byte>.Shared.Return(daveArray);
                throw new DaveEncryptorException(result);
            }

            buffer = daveArray.AsSpan(0, bytesWritten);
        }

        int datagramLength = buffer.Length + encryption.Expansion + 12;

        var datagramArray = ArrayPool<byte>.Shared.Rent(datagramLength);

        WriteDatagram(buffer, new(datagramArray, 0, datagramLength), encryption);

        if (daveArray is not null)
            ArrayPool<byte>.Shared.Return(daveArray);

        connection.Send(new(datagramArray, 0, datagramLength));

        ArrayPool<byte>.Shared.Return(datagramArray);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (client._udpState is not { Connection: var connection, Encryption: var encryption, DaveSession: var session })
        {
            ThrowConnectionNotStarted();
            return;
        }

        byte[]? daveArray = null;
        if (session.GetProtocolVersion() is not 0)
        {
            var daveEncryptor = session.GetEncryptor();
            var length = daveEncryptor.GetMaxCiphertextByteSize(Dave.MediaType.Audio, buffer.Length);
            daveArray = ArrayPool<byte>.Shared.Rent(length);

            int bytesWritten;

            while (true)
            {
                var result = daveEncryptor.Encrypt(Dave.MediaType.Audio, client.Cache.Ssrc, buffer.Span, daveArray, out bytesWritten);

                if (result is Dave.EncryptorResultCode.Success)
                    break;

                if (result is Dave.EncryptorResultCode.MissingKeyRatchet)
                {
                    ArrayPool<byte>.Shared.Return(daveArray);
                    return;
                }

                if (result is Dave.EncryptorResultCode.TooManyAttempts)
                    continue;

                ArrayPool<byte>.Shared.Return(daveArray);
                throw new DaveEncryptorException(result);
            }

            buffer = daveArray.AsMemory(0, bytesWritten);
        }

        int datagramLength = buffer.Length + encryption.Expansion + 12;

        var datagramArray = ArrayPool<byte>.Shared.Rent(datagramLength);

        WriteDatagram(buffer.Span, new(datagramArray, 0, datagramLength), encryption);

        if (daveArray is not null)
            ArrayPool<byte>.Shared.Return(daveArray);

        await connection.SendAsync(new(datagramArray, 0, datagramLength), cancellationToken).ConfigureAwait(false);

        ArrayPool<byte>.Shared.Return(datagramArray);
    }

    private void WriteDatagram(ReadOnlySpan<byte> buffer, Span<byte> datagram, IVoiceEncryption encryption)
    {
        WriteRtpHeader(datagram);
        encryption.Encrypt(buffer, new(datagram));
    }

    private void WriteRtpHeader(Span<byte> datagram)
    {
        datagram[0] = 0b10000000;
        datagram[1] = 0b01111000;
        BinaryPrimitives.WriteUInt16BigEndian(datagram[2..], ++_sequenceNumber);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[4..], _timestamp += Opus.SamplesPerChannel);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[8..], client.Cache.Ssrc);
    }

    [DoesNotReturn]
    private static void ThrowConnectionNotStarted()
    {
        throw new InvalidOperationException("Connection not started.");
    }
}
