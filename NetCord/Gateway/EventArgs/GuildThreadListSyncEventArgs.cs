using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildThreadListSyncEventArgs(JsonGuildThreadListSyncEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public IReadOnlyList<ulong>? ChannelIds { get; } = jsonModel.ChannelIds;

    public IReadOnlyDictionary<ulong, GuildThread> Threads { get; } = dictionaryProvider.CreateDictionary(GuildThreadGenerator.CreateThreads(jsonModel.Threads,
                                                                                                                                             jsonModel.Users,
                                                                                                                                             client),
                                                                                                          thread => thread.Id,
                                                                                                          thread => thread);
}
