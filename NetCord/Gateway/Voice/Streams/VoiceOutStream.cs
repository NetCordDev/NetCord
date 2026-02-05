using System.Security.Cryptography;

namespace NetCord.Gateway.Voice;

internal class VoiceOutStream(VoiceClient client, float frameDuration) : Stream
{
    private readonly uint _samplesPerChannel = (uint)Opus.GetSamplesPerChannel(frameDuration);

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
        client.SendVoice(++_sequenceNumber, _timestamp += _samplesPerChannel, buffer);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return client.SendVoiceAsync(++_sequenceNumber, _timestamp += _samplesPerChannel, buffer, cancellationToken);
    }
}
