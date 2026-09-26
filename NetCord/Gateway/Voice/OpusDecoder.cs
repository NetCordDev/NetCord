using System.Runtime.InteropServices;

using static NetCord.Gateway.Voice.Opus;

namespace NetCord.Gateway.Voice;

public readonly struct OpusDecoder : IDisposable
{
    private readonly OpusDecoderHandle _decoder;
    private readonly VoiceChannels _channels;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">Number of channels to decode.</param>
    public OpusDecoder(VoiceChannels channels)
    {
        var decoder = OpusDecoderCreate(SamplingRate, channels, out var error);

        if (IsError(error))
        {
            decoder.Dispose();
            ThrowOpusException(error);
        }

        _decoder = decoder;
        _channels = channels;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <param name="decodeFec">Whether to decode using forward error correction data, if available.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    public int Decode(ReadOnlySpan<byte> data, Span<byte> pcm, int frameSize, bool decodeFec)
    {
        ValidatePcm(pcm.Length, frameSize, PcmFormat.Short, _channels, nameof(pcm));

        int result = OpusDecode(_decoder, data, data.Length, pcm, frameSize, decodeFec ? 1 : 0);

        ValidateResult(result);

        return result;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <param name="decodeFec">Whether to decode using forward error correction data, if available.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    public int Decode(ReadOnlySpan<byte> data, Span<short> pcm, int frameSize, bool decodeFec)
    {
        return Decode(data, MemoryMarshal.AsBytes(pcm), frameSize, decodeFec);
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <param name="decodeFec">Whether to decode using forward error correction data, if available.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    public int DecodeFloat(ReadOnlySpan<byte> data, Span<byte> pcm, int frameSize, bool decodeFec)
    {
        ValidatePcm(pcm.Length, frameSize, PcmFormat.Float, _channels, nameof(pcm));

        int result = OpusDecodeFloat(_decoder, data, data.Length, pcm, frameSize, decodeFec ? 1 : 0);

        ValidateResult(result);

        return result;
    }

    /// <summary>
    /// Decodes an Opus frame.
    /// </summary>
    /// <param name="data">Input payload. Use <see langword="null"/> to indicate packet loss.</param>
    /// <param name="pcm">Output signal.</param>
    /// <param name="frameSize">Number of samples per channel in the output signal.</param>
    /// <param name="decodeFec">Whether to decode using forward error correction data, if available.</param>
    /// <returns>The number of decoded samples per channel.</returns>
    public int DecodeFloat(ReadOnlySpan<byte> data, Span<float> pcm, int frameSize, bool decodeFec)
    {
        return DecodeFloat(data, MemoryMarshal.AsBytes(pcm), frameSize, decodeFec);
    }

    public void Dispose()
    {
        _decoder.Dispose();
    }
}
