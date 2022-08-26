using NetCord.Rest;

namespace NetCord;

public class StageGuildChannel : TextChannel, IVoiceGuildChannel
{
    public int Bitrate => (int)_jsonModel.Bitrate!;
    public Snowflake? ParentId => _jsonModel.ParentId;
    public string? Topic => _jsonModel.Topic;
    public string RtcRegion => _jsonModel.RtcRegion;
    public VideoQualityMode VideoQualityMode => VideoQualityMode.None;

    public string Name => _jsonModel.Name!;

    public int Position => (int)_jsonModel.Position!;

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public StageGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwrite permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(Snowflake overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion

    #region StageInstance
    public Task<StageInstance> GetStageInstanceAsync(RequestProperties? properties = null) => _client.GetStageInstanceAsync(Id, properties);
    public Task<StageInstance> ModifyStageInstanceAsync(Action<StageInstanceOptions> action, RequestProperties? properties = null) => _client.ModifyStageInstanceAsync(Id, action, properties);
    public Task DeleteStageInstanceAsync(RequestProperties? properties = null) => _client.DeleteStageInstanceAsync(Id, properties);
    #endregion
}
