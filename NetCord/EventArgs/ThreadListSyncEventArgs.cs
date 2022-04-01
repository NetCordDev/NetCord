using System.Collections.Immutable;

namespace NetCord;

public class ThreadListSyncEventArgs
{
    private readonly JsonModels.EventArgs.JsonThreadListSyncEventArgs _jsonEntity;

    internal ThreadListSyncEventArgs(JsonModels.EventArgs.JsonThreadListSyncEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Threads = jsonEntity.Threads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        Users = jsonEntity.Users.Select(u => new ThreadUser(u, client));
    }

    public Snowflake GuildId => _jsonEntity.GuildId;

    public IEnumerable<Snowflake>? ChannelIds => _jsonEntity.ChannelIds;

    public ImmutableDictionary<Snowflake, GuildThread> Threads { get; }

    public IEnumerable<ThreadUser> Users { get; }
}
