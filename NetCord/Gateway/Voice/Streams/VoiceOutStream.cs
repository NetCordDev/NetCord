using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace NetCord.Gateway.Voice;

internal sealed class VoiceOutStream : Stream
{
    private readonly VoiceClient _client;
    private readonly TimeProvider _timeProvider;
    private readonly uint _samplesPerChannel;
    private readonly long _timestampFrequency;

    private ushort _sequenceNumber;
    private uint _timestamp;
    private long _flushedTimestamp;
    private bool _flushed;

    public VoiceOutStream(VoiceClient client, float frameDuration, TimeProvider timeProvider)
    {
        _client = client;
        _timeProvider = timeProvider;
        _samplesPerChannel = (uint)Opus.GetSamplesPerChannel(frameDuration);
        _timestampFrequency = timeProvider.TimestampFrequency;

        RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref _sequenceNumber, 1)));
        RandomNumberGenerator.Fill(MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref _timestamp, 1)));
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    private void FlushInternal()
    {
        if (_flushed)
            return;

        _flushedTimestamp = _timeProvider.GetTimestamp();
        _flushed = true;
    }

    public override void Flush()
    {
        FlushInternal();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        FlushInternal();
        return Task.CompletedTask;
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
        _client.SendVoice(++_sequenceNumber, UpdateTimestamp(), buffer);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _client.SendVoiceAsync(++_sequenceNumber, UpdateTimestamp(), buffer, cancellationToken);
    }

    private uint UpdateTimestamp()
    {
        return _flushed
            ? ResumeTimestamp()
            : (_timestamp += _samplesPerChannel);
    }

    private uint ResumeTimestamp()
    {
        _flushed = false;

        var totalTicks = _timeProvider.GetTimestamp() - _flushedTimestamp;
        var (seconds, ticks) = Math.DivRem(totalTicks, _timestampFrequency);
        var elapsedSamples = (seconds * Opus.SamplingRate) + (ticks * Opus.SamplingRate / _timestampFrequency);

        return _timestamp += (elapsedSamples <= _samplesPerChannel)
            ? _samplesPerChannel
            : (uint)((elapsedSamples + _samplesPerChannel - 1) / _samplesPerChannel * _samplesPerChannel);
    }
}
