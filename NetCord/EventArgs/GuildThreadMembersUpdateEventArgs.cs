using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildThreadMembersUpdateEventArgs
{
    private readonly JsonGuildThreadMembersUpdateEventArgs _jsonEntity;

    internal GuildThreadMembersUpdateEventArgs(JsonGuildThreadMembersUpdateEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.AddedUsers != null)
            AddedUsers = jsonEntity.AddedUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u, client));
    }

    public DiscordId ThreadId => _jsonEntity.ThreadId;

    public DiscordId GuildId => _jsonEntity.GuildId;

    public int MemberCount => _jsonEntity.MemberCount;

    public IReadOnlyDictionary<DiscordId, ThreadUser>? AddedUsers { get; }

    public IEnumerable<DiscordId> RemovedUserIds => _jsonEntity.RemovedUserIds;
}
