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

        using var result = EncryptDave(session, buffer);

        if (result.MissingKeyRatchet)
            return;

        if (!result.IsDisabled)
            buffer = result.Span;

        int datagramLength = buffer.Length + encryption.Expansion + 12;

        var datagramArray = ArrayPool<byte>.Shared.Rent(datagramLength);

        WriteDatagram(buffer, new(datagramArray, 0, datagramLength), encryption);

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

        using var result = EncryptDave(session, buffer.Span);

        if (result.MissingKeyRatchet)
            return;

        if (!result.IsDisabled)
            buffer = result.Memory;

        int datagramLength = buffer.Length + encryption.Expansion + 12;

        var datagramArray = ArrayPool<byte>.Shared.Rent(datagramLength);

        WriteDatagram(buffer.Span, new(datagramArray, 0, datagramLength), encryption);

        await connection.SendAsync(new(datagramArray, 0, datagramLength), cancellationToken).ConfigureAwait(false);

        ArrayPool<byte>.Shared.Return(datagramArray);
    }

    private DaveEncryptionResult EncryptDave(VoiceClient.DaveSession session, ReadOnlySpan<byte> buffer)
    {
        if (!session.IsEnabled)
            return new(default, 0, DaveEncryptionResult.Flags.Disabled);

        var encryptor = session.Encryptor;

        int length = encryptor.GetMaxCiphertextSize(Dave.MediaType.Audio, buffer.Length);

        var array = ArrayPool<byte>.Shared.Rent(length);

        int bytesWritten;

        while (true)
        {
            var result = encryptor.Encrypt(Dave.MediaType.Audio, client.Cache.Ssrc, buffer, array, out bytesWritten);

            if (result is Dave.EncryptorResultCode.Success)
                break;

            if (result is Dave.EncryptorResultCode.MissingKeyRatchet)
            {
                ArrayPool<byte>.Shared.Return(array);

                return new(null, 0, DaveEncryptionResult.Flags.MissingKeyRatchet);
            }

            if (result is Dave.EncryptorResultCode.TooManyAttempts)
                continue;

            ArrayPool<byte>.Shared.Return(array);

            ThrowDaveEncryptorException(result);

            [DoesNotReturn]
            static void ThrowDaveEncryptorException(Dave.EncryptorResultCode result)
            {
                throw new DaveEncryptorException(result);
            }
        }

        return new(array, bytesWritten, default);
    }

    private readonly struct DaveEncryptionResult(byte[]? buffer, int length, DaveEncryptionResult.Flags flags) : IDisposable
    {
        public ReadOnlyMemory<byte> Memory => buffer.AsMemory(0, length);

        public ReadOnlySpan<byte> Span => buffer.AsSpan(0, length);

        public bool IsDisabled => flags.HasFlag(Flags.Disabled);

        public bool MissingKeyRatchet => flags.HasFlag(Flags.MissingKeyRatchet);

        public void Dispose()
        {
            if (buffer is not null)
                ArrayPool<byte>.Shared.Return(buffer);
        }

        [Flags]
        public enum Flags : byte
        {
            Disabled = 1 << 0,
            MissingKeyRatchet = 1 << 1,
        }
    };

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
