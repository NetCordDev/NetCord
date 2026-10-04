using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a standard voice channel within a guild.
/// </summary>
public partial class VoiceGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client), IVoiceGuildChannel
{
    public int Bitrate { get; } = jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit { get; } = jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion { get; } = jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode { get; } = jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);
}
