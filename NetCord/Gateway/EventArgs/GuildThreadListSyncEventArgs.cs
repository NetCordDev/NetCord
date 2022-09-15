using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadListSyncEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>
{
    JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs _jsonModel;

    public GuildThreadListSyncEventArgs(JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Threads = jsonModel.Threads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        Users = jsonModel.Users.Select(u => new ThreadUser(u, client));
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public IReadOnlyList<Snowflake>? ChannelIds => _jsonModel.ChannelIds;

    public ImmutableDictionary<Snowflake, GuildThread> Threads { get; }

    public IEnumerable<ThreadUser> Users { get; }
}
