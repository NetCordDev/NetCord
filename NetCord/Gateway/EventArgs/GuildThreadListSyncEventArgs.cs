using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadListSyncEventArgs(JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>
{
    JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public IReadOnlyList<ulong>? ChannelIds => jsonModel.ChannelIds;

    public ImmutableDictionary<ulong, GuildThread> Threads { get; } = jsonModel.Threads.ToImmutableDictionary(t => t.Id, t => GuildThread.CreateFromJson(t, client));

    public IReadOnlyList<ThreadUser> Users { get; } = jsonModel.Users.Select(u => new ThreadUser(u, client)).ToArray();
}
