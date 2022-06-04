using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal static class Opus
{
    public const int SamplingRate = 48_000;
    public const int Channels = 2;
    public const int FrameMillis = 20;
    public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;
    public const int SampleBytes = sizeof(short) * Channels;
    public const int FrameSize = FrameSamplesPerChannel * SampleBytes;

    /// <param name="Fs">Sampling rate of input signal (Hz) This must be one of 8000, 12000, 16000, 24000, or 48000.</param>
    /// <param name="channels">Number of channels (1 or 2) in input signal.</param>
    /// <param name="application">Coding mode.</param>
    /// <param name="error"></param>
    /// <returns>Pointer to OpusEncoder.</returns>
    [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
    public static extern OpusEncoderHandle OpusEncoderCreate(int Fs, int channels, OpusApplication application, out OpusError error);

    [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void OpusEncoderDestroy(OpusEncoderHandle st);

    /// <param name="st">Encoder state.</param>
    /// <param name="pcm">Input signal (interleaved if 2 channels). Length is <paramref name="frame_size"/>*channels*<see langword="sizeof"/>(<see langword="short"/>).</param>
    /// <param name="frame_size">Number of samples per channel in the input signal. This must be an Opus frame size for the encoder's sampling rate. For example, at 48 kHz the permitted values are 120, 240, 480, 960, 1920, and 2880. Passing in a duration of less than 10 ms (480 samples at 48 kHz) will prevent the encoder from using the LPC or hybrid modes.</param>
    /// <param name="data">Output payload. This must contain storage for at least <paramref name="max_data_bytes"/>.</param>
    /// <param name="max_data_bytes">Size of the allocated memory for the output payload. This may be used to impose an upper limit on the instant bitrate, but should not be used as the only bitrate control.</param>
    /// <returns>The length of the encoded packet (in bytes) on success or a negative error code (see Error codes) on failure.</returns>
    [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe int OpusEncode(OpusEncoderHandle st, short* pcm, int frame_size, byte* data, int max_data_bytes);

    [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
    public static extern OpusDecoderHandle OpusDecoderCreate(int Fs, int channels, out OpusError error);

    [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void OpusDecoderDestroy(OpusDecoderHandle st);

    [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe int OpusDecode(OpusDecoderHandle st, byte* data, int len, short* pcm, int frame_size, int decode_fec);
}