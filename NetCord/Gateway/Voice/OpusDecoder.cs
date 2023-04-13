using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public class OpusDecoder : IDisposable
{
    private readonly OpusDecoderHandle _decoder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">Number of channels to decode.</param>
    /// <exception cref="OpusException"></exception>
    public OpusDecoder(VoiceChannels channels)
    {
        var decoder = Opus.OpusDecoderCreate(Opus.SamplingRate, channels, out var error);
        if (error != 0)
            throw new OpusException(error);

        _decoder = decoder;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <exception cref="OpusException"></exception>
    public void Decode(ReadOnlySpan<byte> data, Span<byte> pcm)
    {
        int result = Opus.OpusDecode(_decoder, ref MemoryMarshal.GetReference(data), data.Length, ref MemoryMarshal.GetReference(pcm), Opus.SamplesPerChannel, 0);

        if (result < 0)
            throw new OpusException((OpusError)result);
    }

    public void Dispose()
    {
        _decoder.Dispose();
    }
}
