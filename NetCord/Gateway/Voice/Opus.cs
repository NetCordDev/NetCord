using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public static partial class Opus
{
    public const int SamplingRate = 48_000;
    public const int FrameDuration = 20;
    public const int SamplesPerChannel = SamplingRate * FrameDuration / 1000;
    public const int MaxOpusBitrate = 510_000;
    public const int MaxOpusFrameLength = MaxOpusBitrate / 8 * FrameDuration / 1000;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format">Format of PCM.</param>
    /// <param name="channels">Number of channels.</param>
    /// <returns></returns>
    public static int GetFrameSize(PcmFormat format, VoiceChannels channels) => SamplesPerChannel * (byte)format * (byte)channels;

    [LibraryImport("opus", EntryPoint = "opus_encoder_create")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial OpusEncoderHandle OpusEncoderCreate(int Fs, VoiceChannels channels, OpusApplication application, out OpusError error);

    [LibraryImport("opus", EntryPoint = "opus_encoder_destroy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial void OpusEncoderDestroy(nint st);

    [LibraryImport("opus", EntryPoint = "opus_encode")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusEncode(OpusEncoderHandle st, ref byte pcm, int frame_size, ref byte data, int max_data_bytes);

    [LibraryImport("opus", EntryPoint = "opus_encode_float")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusEncodeFloat(OpusEncoderHandle st, ref byte pcm, int frame_size, ref byte data, int max_data_bytes);

    [LibraryImport("opus", EntryPoint = "opus_decoder_create")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial OpusDecoderHandle OpusDecoderCreate(int Fs, VoiceChannels channels, out OpusError error);

    [LibraryImport("opus", EntryPoint = "opus_decoder_destroy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial void OpusDecoderDestroy(nint st);

    [LibraryImport("opus", EntryPoint = "opus_decode")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusDecode(OpusDecoderHandle st, ref byte data, int len, ref byte pcm, int frame_size, int decode_fec);

    [LibraryImport("opus", EntryPoint = "opus_decode_float")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusDecodeFloat(OpusDecoderHandle st, ref byte data, int len, ref byte pcm, int frame_size, int decode_fec);
}
