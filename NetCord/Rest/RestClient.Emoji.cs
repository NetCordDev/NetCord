namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, GuildEmoji>> GetGuildEmojisAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmojiArray).ConfigureAwait(false)).ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    public async Task<GuildEmoji> GetGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);

    public async Task<GuildEmoji> CreateGuildEmojiAsync(ulong guildId, GuildEmojiProperties guildEmojiProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildEmojiProperties>(guildEmojiProperties, Serialization.Default.GuildEmojiProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/emojis", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildEmoji> ModifyGuildEmojiAsync(ulong guildId, ulong emojiId, Action<GuildEmojiOptions> action, RequestProperties? properties = null)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        using (HttpContent content = new JsonContent<GuildEmojiOptions>(guildEmojiOptions, Serialization.Default.GuildEmojiOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    public Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", null, new(guildId), properties);
}
