using System.Collections.Immutable;

namespace NetCord;

public class ThreadListSyncEventArgs : IJsonModel<JsonModels.EventArgs.JsonThreadListSyncEventArgs>
{
    JsonModels.EventArgs.JsonThreadListSyncEventArgs IJsonModel<JsonModels.EventArgs.JsonThreadListSyncEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonThreadListSyncEventArgs _jsonModel;

    public ThreadListSyncEventArgs(JsonModels.EventArgs.JsonThreadListSyncEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Threads = jsonModel.Threads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        Users = jsonModel.Users.Select(u => new ThreadUser(u, client));
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public IEnumerable<Snowflake>? ChannelIds => _jsonModel.ChannelIds;

    public ImmutableDictionary<Snowflake, GuildThread> Threads { get; }

    public IEnumerable<ThreadUser> Users { get; }
}
