using NetCord.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadUsersUpdateEventArgs : IJsonModel<JsonGuildThreadUsersUpdateEventArgs>
{
    JsonGuildThreadUsersUpdateEventArgs IJsonModel<JsonGuildThreadUsersUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonGuildThreadUsersUpdateEventArgs _jsonModel;

    public GuildThreadUsersUpdateEventArgs(JsonGuildThreadUsersUpdateEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.AddedUsers != null)
            AddedUsers = jsonModel.AddedUsers.ToDictionary(u => u.UserId, u => new AddedThreadUser(u, GuildId, client));
    }

    public ulong ThreadId => _jsonModel.ThreadId;

    public ulong GuildId => _jsonModel.GuildId;

    public int UserCount => _jsonModel.UserCount;

    public IReadOnlyDictionary<ulong, AddedThreadUser>? AddedUsers { get; }

    public IReadOnlyList<ulong> RemovedUserIds => _jsonModel.RemovedUserIds;
}
