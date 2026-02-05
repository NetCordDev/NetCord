namespace NetCord.Gateway.Voice;

public readonly struct OpusDecoder : IDisposable
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
        if (error is not 0)
            throw new OpusException(error);

        _decoder = decoder;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    /// <exception cref="OpusException"></exception>
    public int Decode(ReadOnlySpan<byte> data, Span<byte> pcm, int frameSize)
    {
        int result = Opus.OpusDecode(_decoder, data, data.Length, pcm, frameSize, 0);

        if (result < 0)
            throw new OpusException((OpusError)result);

        return result;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    /// <exception cref="OpusException"></exception>
    public int DecodeFloat(ReadOnlySpan<byte> data, Span<byte> pcm, int frameSize)
    {
        int result = Opus.OpusDecodeFloat(_decoder, data, data.Length, pcm, frameSize, 0);

        if (result < 0)
            throw new OpusException((OpusError)result);

        return result;
    }

    public void Dispose()
    {
        _decoder.Dispose();
    }
}
