using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildStickersUpdateEventArgs(JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public IReadOnlyDictionary<ulong, GuildSticker> Stickers { get; } = dictionaryProvider.CreateDictionary(jsonModel.Stickers, s => s.Id, s => new GuildSticker(s, client));
}
