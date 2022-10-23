using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, GuildEmoji>> GetGuildEmojisAsync(ulong guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonEmoji.JsonEmojiArraySerializerContext.WithOptions.JsonEmojiArray).ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    public async Task<GuildEmoji> GetGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji), guildId, this);

    public async Task<GuildEmoji> CreateGuildEmojiAsync(ulong guildId, GuildEmojiProperties guildEmojiProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/emojis", new JsonContent<GuildEmojiProperties>(guildEmojiProperties, GuildEmojiProperties.GuildEmojiPropertiesSerializerContext.WithOptions.GuildEmojiProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji), guildId, this);

    public async Task<GuildEmoji> ModifyGuildEmojiAsync(ulong guildId, ulong emojiId, Action<GuildEmojiOptions> action, RequestProperties? properties = null)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.ModifyGuildEmoji, guildId), new JsonContent<GuildEmojiOptions>(guildEmojiOptions, GuildEmojiOptions.GuildEmojiOptionsSerializerContext.WithOptions.GuildEmojiOptions), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonEmoji.JsonEmojiSerializerContext.WithOptions.JsonEmoji), guildId, this);
    }

    public Task DeleteGuildEmojiAsync(ulong guildId, ulong emojiId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.DeleteGuildEmoji, guildId), properties);
}
