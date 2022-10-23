using NetCord.Rest;

namespace NetCord;

public abstract class GuildThread : TextChannel
{
    public ulong GuildId => _jsonModel.GuildId.GetValueOrDefault();
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public GuildThreadMetadata Metadata { get; }
    public ThreadSelfUser? CurrentUser { get; }
    public ulong ParentId => _jsonModel.ParentId.GetValueOrDefault();
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public string Name => _jsonModel.Name!;
    public int MessageCount => _jsonModel.MessageCount.GetValueOrDefault();
    public int UserCount => _jsonModel.UserCount.GetValueOrDefault();
    public int DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration.GetValueOrDefault();
    public ChannelFlags Flags => _jsonModel.Flags.GetValueOrDefault();
    public int TotalMessageSent => _jsonModel.TotalMessageSent.GetValueOrDefault();

    public GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Metadata = new(jsonModel.Metadata!);
        if (jsonModel.CurrentUser != null)
            CurrentUser = new(jsonModel.CurrentUser);
    }

    #region Channel
    public async Task<GuildThread> ModifyAsync(Action<GuildThreadOptions> action, RequestProperties? properties = null) => (GuildThread)await _client.ModifyGuildThreadAsync(Id, action, properties).ConfigureAwait(false);
    public Task JoinAsync(RequestProperties? properties = null) => _client.JoinGuildThreadAsync(Id, properties);
    public Task AddUserAsync(ulong userId, RequestProperties? properties = null) => _client.AddGuildThreadUserAsync(Id, userId, properties);
    public Task LeaveAsync(RequestProperties? properties = null) => _client.LeaveGuildThreadAsync(Id, properties);
    public Task DeleteUserAsync(ulong userId, RequestProperties? properties = null) => _client.DeleteGuildThreadUserAsync(Id, userId, properties);
    public Task<ThreadUser> GetUserAsync(ulong userId, RequestProperties? properties = null) => _client.GetGuildThreadUserAsync(Id, userId, properties);
    public Task<IReadOnlyDictionary<ulong, ThreadUser>> GetGuildThreadUsersAsync(RequestProperties? properties = null) => _client.GetGuildThreadUsersAsync(Id, properties);
    #endregion
}
