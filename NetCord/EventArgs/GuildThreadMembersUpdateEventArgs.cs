using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildThreadMembersUpdateEventArgs : IJsonModel<JsonGuildThreadMembersUpdateEventArgs>
{
    JsonGuildThreadMembersUpdateEventArgs IJsonModel<JsonGuildThreadMembersUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonGuildThreadMembersUpdateEventArgs _jsonModel;

    public GuildThreadMembersUpdateEventArgs(JsonGuildThreadMembersUpdateEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.AddedUsers != null)
            AddedUsers = jsonModel.AddedUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u, client));
    }

    public Snowflake ThreadId => _jsonModel.ThreadId;

    public Snowflake GuildId => _jsonModel.GuildId;

    public int MemberCount => _jsonModel.MemberCount;

    public IReadOnlyDictionary<Snowflake, ThreadUser>? AddedUsers { get; }

    public IEnumerable<Snowflake> RemovedUserIds => _jsonModel.RemovedUserIds;
}
