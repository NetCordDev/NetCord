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
            AddedUsers = jsonModel.AddedUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u, client));
    }

    public Snowflake ThreadId => _jsonModel.ThreadId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public int UserCount => _jsonModel.UserCount;

    public IReadOnlyDictionary<Snowflake, ThreadUser>? AddedUsers { get; }

    public IReadOnlyList<Snowflake> RemovedUserIds => _jsonModel.RemovedUserIds;
}