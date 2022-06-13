using System.Collections.Immutable;

namespace NetCord;

public class GuildStickersUpdateEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs _jsonModel;

    public GuildStickersUpdateEventArgs(JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Stickers = jsonModel.Stickers.ToImmutableDictionary(s => s.Id, s => new GuildSticker(s, client));
    }

    public Snowflake GuildId => _jsonModel.GuildId;

    public ImmutableDictionary<Snowflake, GuildSticker> Stickers { get; }
}
