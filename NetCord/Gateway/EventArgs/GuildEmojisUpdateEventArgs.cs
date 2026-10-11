using NetCord.Gateway.JsonModels.EventArgs;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildEmojisUpdateEventArgs(JsonGuildEmojisUpdateEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public IReadOnlyDictionary<ulong, GuildEmoji> Emojis { get; } = CreateEmojis(jsonModel, client, dictionaryProvider);

    private static IReadOnlyDictionary<ulong, GuildEmoji> CreateEmojis(JsonGuildEmojisUpdateEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.GuildId;

        return dictionaryProvider.CreateDictionary(jsonModel.Emojis, e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, client));
    }
}
