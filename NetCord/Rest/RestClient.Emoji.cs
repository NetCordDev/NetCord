using NetCord.JsonModels;
using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, GuildEmoji>> GetGuildEmojisAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", properties).ConfigureAwait(false)).ToObjectAsync(JsonEmoji.JsonEmojiArraySerializerContext.WithOptions.JsonEmojiArray).ConfigureAwait(false)).ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    public async Task<GuildEmoji> GetGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji).ConfigureAwait(false), guildId, this);

    public async Task<GuildEmoji> CreateGuildEmojiAsync(ulong guildId, GuildEmojiProperties guildEmojiProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildEmojiProperties>(guildEmojiProperties, GuildEmojiProperties.GuildEmojiPropertiesSerializerContext.WithOptions.GuildEmojiProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/emojis", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildEmoji> ModifyGuildEmojiAsync(ulong guildId, ulong emojiId, Action<GuildEmojiOptions> action, RequestProperties? properties = null)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        using (HttpContent content = new JsonContent<GuildEmojiOptions>(guildEmojiOptions, GuildEmojiOptions.GuildEmojiOptionsSerializerContext.WithOptions.GuildEmojiOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.ModifyGuildEmoji, guildId), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji).ConfigureAwait(false), guildId, this);
    }

    public Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.DeleteGuildEmoji, guildId), properties);
}
