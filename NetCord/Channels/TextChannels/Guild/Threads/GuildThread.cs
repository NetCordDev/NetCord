using NetCord.Rest;

namespace NetCord;

public abstract class GuildThread : TextChannel
{
    public ulong GuildId => _jsonModel.GuildId.GetValueOrDefault();
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public ulong ParentId => _jsonModel.ParentId.GetValueOrDefault();
    public int MessageCount => _jsonModel.MessageCount.GetValueOrDefault();
    public int UserCount => _jsonModel.UserCount.GetValueOrDefault();
    public GuildThreadMetadata Metadata { get; }
    public ThreadSelfUser? CurrentUser { get; }
    public int TotalMessageSent => _jsonModel.TotalMessageSent.GetValueOrDefault();

    public GuildThread(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Metadata = new(jsonModel.Metadata!);
        if (jsonModel.CurrentUser != null)
            CurrentUser = new(jsonModel.CurrentUser);
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    #region Channel
    public async Task<GuildThread> ModifyAsync(Action<GuildThreadOptions> action, RequestProperties? properties = null) => (GuildThread)await _client.ModifyGuildThreadAsync(Id, action, properties).ConfigureAwait(false);
    public Task JoinAsync(RequestProperties? properties = null) => _client.JoinGuildThreadAsync(Id, properties);
    public Task AddUserAsync(ulong userId, RequestProperties? properties = null) => _client.AddGuildThreadUserAsync(Id, userId, properties);
    public Task LeaveAsync(RequestProperties? properties = null) => _client.LeaveGuildThreadAsync(Id, properties);
    public Task DeleteUserAsync(ulong userId, RequestProperties? properties = null) => _client.DeleteGuildThreadUserAsync(Id, userId, properties);
    public Task<ThreadUser> GetUserAsync(ulong userId, bool withGuildUser = false, RequestProperties? properties = null) => _client.GetGuildThreadUserAsync(Id, userId, withGuildUser, properties);
    public IAsyncEnumerable<ThreadUser> GetGuildThreadUsersAsync(bool withGuildUsers = false, RequestProperties ? properties = null) => _client.GetGuildThreadUsersAsync(Id, withGuildUsers, properties);
    public IAsyncEnumerable<ThreadUser> GetGuildThreadUsersAfterAsync(ulong after, bool withGuildUsers = false, RequestProperties? properties = null) => _client.GetGuildThreadUsersAfterAsync(Id, after, withGuildUsers, properties);
    #endregion
}
