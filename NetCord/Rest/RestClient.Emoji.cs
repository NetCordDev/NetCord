using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildEmoji>> GetGuildEmojisAsync(ulong guildId, RestRequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmojiArray).ConfigureAwait(false)).ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildEmoji), nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public async Task<GuildEmoji> GetGuildEmojiAsync(ulong guildId, ulong emojiId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildEmoji> CreateGuildEmojiAsync(ulong guildId, GuildEmojiProperties guildEmojiProperties, RestRequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildEmojiProperties>(guildEmojiProperties, Serialization.Default.GuildEmojiProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/emojis", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildEmoji), nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public async Task<GuildEmoji> ModifyGuildEmojiAsync(ulong guildId, ulong emojiId, Action<GuildEmojiOptions> action, RestRequestProperties? properties = null)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        using (HttpContent content = new JsonContent<GuildEmojiOptions>(guildEmojiOptions, Serialization.Default.GuildEmojiOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildEmoji), nameof(GuildEmoji.GuildId), nameof(GuildEmoji.Id))]
    public Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, RestRequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties);
}
