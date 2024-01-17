using NetCord.Rest;

namespace NetCord;

public class StageGuildChannel : TextChannel, IVoiceGuildChannel
{
    public StageGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
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

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion

    #region StageInstance
    public Task<StageInstance> GetStageInstanceAsync(RequestProperties? properties = null) => _client.GetStageInstanceAsync(Id, properties);
    public Task<StageInstance> ModifyStageInstanceAsync(Action<StageInstanceOptions> action, RequestProperties? properties = null) => _client.ModifyStageInstanceAsync(Id, action, properties);
    public Task DeleteStageInstanceAsync(RequestProperties? properties = null) => _client.DeleteStageInstanceAsync(Id, properties);
    #endregion
}
