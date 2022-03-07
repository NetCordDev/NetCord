using System.Collections.Immutable;

namespace NetCord;

public class GuildStickersUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs _jsonEntity;

    internal GuildStickersUpdateEventArgs(JsonModels.EventArgs.JsonGuildStickersUpdateEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Stickers = jsonEntity.Stickers.ToImmutableDictionary(s => s.Id, s => new GuildSticker(s, client));
    }

    public DiscordId GuildId => _jsonEntity.GuildId;

    public ImmutableDictionary<DiscordId, GuildSticker> Stickers { get; }
}
