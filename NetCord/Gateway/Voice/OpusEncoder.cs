using System.Runtime.InteropServices;

using static NetCord.Gateway.Voice.Opus;

namespace NetCord.Gateway.Voice;

public readonly struct OpusEncoder : IDisposable
{
    private readonly OpusEncoderHandle _encoder;
    private readonly VoiceChannels _channels;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">Number of channels in input signal.</param>
    /// <param name="application">Opus coding mode.</param>
    public OpusEncoder(VoiceChannels channels, OpusApplication application)
    {
        var encoder = OpusEncoderCreate(SamplingRate, channels, application, out var error);

        if (IsError(error))
        {
            encoder.Dispose();
            ThrowOpusException(error);
        }

        _encoder = encoder;
        _channels = channels;
    }

    /// <summary>
    /// Encodes an Opus frame.
    /// </summary>
    /// <param name="pcm">Input signal.</param>
    /// <param name="frameSize">Number of samples per channel in the input signal.</param>
    /// <param name="data">Output payload.</param>
    /// <returns>The length of the encoded packet.</returns>
    public int Encode(ReadOnlySpan<byte> pcm, int frameSize, Span<byte> data)
    {
        ValidatePcm(pcm.Length, frameSize, PcmFormat.Short, _channels, nameof(pcm));

        int result = OpusEncode(_encoder, pcm, frameSize, data, data.Length);

        ValidateResult(result);

        return result;
    }

    /// <summary>
    /// Encodes an Opus frame.
    /// </summary>
    /// <param name="pcm">Input signal.</param>
    /// <param name="frameSize">Number of samples per channel in the input signal.</param>
    /// <param name="data">Output payload.</param>
    /// <returns>The length of the encoded packet.</returns>
    public int Encode(ReadOnlySpan<short> pcm, int frameSize, Span<byte> data)
    {
        return Encode(MemoryMarshal.AsBytes(pcm), frameSize, data);
    }

    /// <summary>
    /// Encodes an Opus frame.
    /// </summary>
    /// <param name="pcm">Input signal.</param>
    /// <param name="frameSize">Number of samples per channel in the input signal.</param>
    /// <param name="data">Output payload.</param>
    /// <returns>The length of the encoded packet.</returns>
    public int EncodeFloat(ReadOnlySpan<byte> pcm, int frameSize, Span<byte> data)
    {
        ValidatePcm(pcm.Length, frameSize, PcmFormat.Float, _channels, nameof(pcm));

        int result = OpusEncodeFloat(_encoder, pcm, frameSize, data, data.Length);

        ValidateResult(result);

        return result;
    }

    /// <summary>
    /// Encodes an Opus frame.
    /// </summary>
    /// <param name="pcm">Input signal.</param>
    /// <param name="frameSize">Number of samples per channel in the input signal.</param>
    /// <param name="data">Output payload.</param>
    /// <returns>The length of the encoded packet.</returns>
    public int EncodeFloat(ReadOnlySpan<float> pcm, int frameSize, Span<byte> data)
    {
        return EncodeFloat(MemoryMarshal.AsBytes(pcm), frameSize, data);
    }

    public void Dispose()
    {
        _encoder.Dispose();
    }
}
