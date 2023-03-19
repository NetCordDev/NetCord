using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public static class Opus
{
    public const int SamplingRate = 48_000;
    public const int FrameDuration = 20;
    public const int SamplesPerChannel = SamplingRate / 1000 * FrameDuration;
    public const int MaxOpusFrameLength = (255 * 4) + 255; // https://www.rfc-editor.org/rfc/rfc6716#section-3.2.1

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">Number of channels.</param>
    /// <returns></returns>
    public static int GetFrameSize(VoiceChannels channels) => SamplesPerChannel * sizeof(short) * (int)channels;

    [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
    internal static extern OpusEncoderHandle OpusEncoderCreate(int Fs, VoiceChannels channels, OpusApplication application, out OpusError error);

    [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void OpusEncoderDestroy(nint st);

    [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int OpusEncode(OpusEncoderHandle st, short* pcm, int frame_size, byte* data, int max_data_bytes);

    [DllImport("opus", EntryPoint = "opus_decoder_create", CallingConvention = CallingConvention.Cdecl)]
    internal static extern OpusDecoderHandle OpusDecoderCreate(int Fs, VoiceChannels channels, out OpusError error);

    [DllImport("opus", EntryPoint = "opus_decoder_destroy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void OpusDecoderDestroy(nint st);

    [DllImport("opus", EntryPoint = "opus_decode", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int OpusDecode(OpusDecoderHandle st, byte* data, int len, short* pcm, int frame_size, int decode_fec);
}
