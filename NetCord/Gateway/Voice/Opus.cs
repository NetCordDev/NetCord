using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public static partial class Opus
{
    public const int SamplingRate = 48_000;

    public const int MaxBitrate = 510_000;

    public const float DefaultFrameDuration = 20;
    public const float MaxFrameDuration = 120;

    /// <summary>
    /// Calculates the number of samples per channel in an Opus audio frame.
    /// </summary>
    /// <param name="frameDuration">Frame duration in milliseconds.</param>
    /// <returns></returns>
    public static int GetSamplesPerChannel(float frameDuration) => (int)(frameDuration * 2) * SamplingRate / 1000 / 2;

    /// <summary>
    /// Calculates the required buffer size, in bytes, for a PCM audio frame.
    /// </summary>
    /// <param name="samplesPerChannel">The number of samples per channel.</param>
    /// <param name="format">The PCM format to use.</param>
    /// <param name="channels">The number of channels to use.</param>
    /// <returns>The size of the buffer, in bytes, required to store the audio frame for the given duration, format, and channel count.</returns>
    public static int GetFrameBufferSize(int samplesPerChannel, PcmFormat format, VoiceChannels channels) => samplesPerChannel * (byte)format * (byte)channels;

    /// <summary>
    /// Calculates the maximum possible size, in bytes, of an Opus audio frame.
    /// </summary>
    /// <param name="frameDuration">The duration of the Opus frame, in milliseconds.</param>
    /// <returns>The maximum number of bytes that an Opus frame of the specified duration can occupy.</returns>
    public static int GetMaxOpusFrameSize(float frameDuration) => (int)(frameDuration * 2) * (MaxBitrate / 8 / 1000) / 2;

    [LibraryImport("opus", EntryPoint = "opus_encoder_create")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial OpusEncoderHandle OpusEncoderCreate(int Fs, VoiceChannels channels, OpusApplication application, out OpusError error);

    [LibraryImport("opus", EntryPoint = "opus_encode")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusEncode(OpusEncoderHandle st, ReadOnlySpan<byte> pcm, int frame_size, Span<byte> data, int max_data_bytes);

    [LibraryImport("opus", EntryPoint = "opus_encode_float")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusEncodeFloat(OpusEncoderHandle st, ReadOnlySpan<byte> pcm, int frame_size, Span<byte> data, int max_data_bytes);

    [LibraryImport("opus", EntryPoint = "opus_encoder_destroy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial void OpusEncoderDestroy(nint st);

    [LibraryImport("opus", EntryPoint = "opus_decoder_create")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial OpusDecoderHandle OpusDecoderCreate(int Fs, VoiceChannels channels, out OpusError error);

    [LibraryImport("opus", EntryPoint = "opus_decode")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusDecode(OpusDecoderHandle st, ReadOnlySpan<byte> data, int len, Span<byte> pcm, int frame_size, int decode_fec);

    [LibraryImport("opus", EntryPoint = "opus_decode_float")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int OpusDecodeFloat(OpusDecoderHandle st, ReadOnlySpan<byte> data, int len, Span<byte> pcm, int frame_size, int decode_fec);

    [LibraryImport("opus", EntryPoint = "opus_decoder_destroy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial void OpusDecoderDestroy(nint st);
}
