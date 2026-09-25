using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a standard voice channel within a guild.
/// </summary>
public partial class VoiceGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client), IVoiceGuildChannel
{
    public int Bitrate => _jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit => _jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion => _jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode => _jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);
}
