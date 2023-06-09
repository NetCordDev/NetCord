using NetCord.JsonModels;

namespace NetCord;

public class VoiceAttachment : Attachment
{
    public VoiceAttachment(JsonAttachment jsonModel) : base(jsonModel)
    {
    }

    /// <summary>
    /// The duration of the audio file.
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromSeconds(_jsonModel.DurationSeconds.GetValueOrDefault());

    /// <summary>
    /// Byte array representing a sampled waveform. It is intended to be a preview of the entire voice message. Clients sample the recording at most once per 100 milliseconds, but will downsample so that no more than 256 datapoints are in the waveform.
    /// </summary>
    public IReadOnlyList<byte> Waveform => _jsonModel.Waveform!;
}
