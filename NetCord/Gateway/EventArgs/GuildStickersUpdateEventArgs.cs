using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildStickersUpdateEventArgs(JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) : IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public IReadOnlyDictionary<ulong, GuildSticker> Stickers { get; } = dictionaryProvider.CreateDictionary(jsonModel.Stickers, s => s.Id, s => new GuildSticker(s, client));
}
