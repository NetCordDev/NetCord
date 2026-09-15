using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a stage channel within a guild.
/// </summary>
public partial class StageGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : TextGuildChannel(jsonModel, guildId, client), IVoiceGuildChannel
{
    public int Bitrate => jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit => jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion => jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode => jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);
}
