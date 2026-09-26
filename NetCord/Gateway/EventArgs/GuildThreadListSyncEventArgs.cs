using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadListSyncEventArgs(JsonGuildThreadListSyncEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) : IJsonModel<JsonGuildThreadListSyncEventArgs>
{
    JsonGuildThreadListSyncEventArgs IJsonModel<JsonGuildThreadListSyncEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public IReadOnlyList<ulong>? ChannelIds => jsonModel.ChannelIds;

    public IReadOnlyDictionary<ulong, GuildThread> Threads { get; } = dictionaryProvider.CreateDictionary(GuildThreadGenerator.CreateThreads(jsonModel.Threads,
                                                                                                                                             jsonModel.Users,
                                                                                                                                             client),
                                                                                                          thread => thread.Id,
                                                                                                          thread => thread);
}
