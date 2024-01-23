using NetCord.Rest;

namespace NetCord;

public partial class VoiceGuildChannel : TextChannel, IVoiceGuildChannel
{
    public VoiceGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();
    public int Bitrate => _jsonModel.Bitrate.GetValueOrDefault();
    public int UserLimit => _jsonModel.UserLimit.GetValueOrDefault();
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong? ParentId => _jsonModel.ParentId;
    public string? RtcRegion => _jsonModel.RtcRegion;
    public VideoQualityMode VideoQualityMode => _jsonModel.VideoQualityMode.GetValueOrDefault(VideoQualityMode.Auto);
}
