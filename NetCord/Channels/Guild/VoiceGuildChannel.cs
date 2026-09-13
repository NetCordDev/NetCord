using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a standard voice channel within a guild.
/// </summary>
internal partial class VoiceGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : GuildMessageChannelBase(jsonModel, guildId, client), IVoiceGuildChannel
{
    public int Bitrate => _jsonModel.Bitrate.GetValueOrDefault();

    public int UserLimit => _jsonModel.UserLimit.GetValueOrDefault();

    public string? RtcRegion => _jsonModel.RtcRegion;

    public VideoQualityMode VideoQualityMode => _jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);

    public int Slowmode => throw new NotImplementedException();

    public ulong? ParentId => throw new NotImplementedException();

    public string Name => throw new NotImplementedException();

    public int Position => throw new NotImplementedException();

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites => throw new NotImplementedException();
}
