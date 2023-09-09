using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public class OpusEncoder : IDisposable
{
    private readonly OpusEncoderHandle _encoder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">Number of channels in input signal.</param>
    /// <param name="application">Opus coding mode.</param>
    /// <exception cref="OpusException"></exception>
    public OpusEncoder(VoiceChannels channels, OpusApplication application)
    {
        var encoder = Opus.OpusEncoderCreate(Opus.SamplingRate, channels, application, out var error);
        if (error != 0)
            throw new OpusException(error);

        _encoder = encoder;
    }

    /// <summary>
    /// Encodes an Opus frame.
    /// </summary>
    /// <param name="pcm">Input signal.</param>
    /// <param name="data">Output payload.</param>
    /// <returns>The length of the encoded packet.</returns>
    /// <exception cref="OpusException"></exception>
    public int Encode(ReadOnlySpan<byte> pcm, Span<byte> data)
    {
        int result = Opus.OpusEncode(_encoder, ref MemoryMarshal.GetReference(pcm), Opus.SamplesPerChannel, ref MemoryMarshal.GetReference(data), data.Length);

        if (result < 0)
            throw new OpusException((OpusError)result);

        return result;
    }

    public void Dispose()
    {
        _encoder.Dispose();
    }
}
