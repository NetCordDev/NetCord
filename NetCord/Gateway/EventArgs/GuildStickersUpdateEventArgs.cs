using System.Collections.Immutable;

using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildStickersUpdateEventArgs(JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs jsonModel, RestClient client) : IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>.JsonModel => jsonModel;

    public ulong GuildId => jsonModel.GuildId;

    public ImmutableDictionary<ulong, GuildSticker> Stickers { get; } = jsonModel.Stickers.ToImmutableDictionary(s => s.Id, s => new GuildSticker(s, client));
}
