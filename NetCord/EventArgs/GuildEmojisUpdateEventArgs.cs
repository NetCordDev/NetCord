using System.Collections.Immutable;

namespace NetCord;

public class GuildEmojisUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs _jsonEntity;

    internal GuildEmojisUpdateEventArgs(JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Emojis = jsonEntity.Emojis.ToImmutableDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, GuildId, client));
    }

    public DiscordId GuildId => _jsonEntity.GuildId;

    public ImmutableDictionary<DiscordId, GuildEmoji> Emojis { get; }
}