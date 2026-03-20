using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadListSyncEventArgs(JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) : IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>
{
    JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildThreadListSyncEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public IReadOnlyList<ulong>? ChannelIds => jsonModel.ChannelIds;

    public IReadOnlyDictionary<ulong, GuildThread> Threads { get; } = dictionaryProvider.CreateDictionary(jsonModel.Threads, t => t.Id, t => GuildThread.CreateFromJson(t, client));

    public IReadOnlyList<ThreadUser> Users { get; } = jsonModel.Users.Select(u => new ThreadUser(u, client)).ToArray();
}
