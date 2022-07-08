using NetCord.Rest.RateLimits;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, GuildEmoji>> GetGuildEmojisAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis", properties).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonEmoji>>().ToDictionary(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, this));

    public async Task<GuildEmoji> GetGuildEmojiAsync(Snowflake guildId, Snowflake emojiId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/emojis/{emojiId}", properties).ConfigureAwait(false)).ToObject<JsonModels.JsonEmoji>(), guildId, this);

    public async Task<GuildEmoji> CreateGuildEmojiAsync(Snowflake guildId, GuildEmojiProperties guildEmojiProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/emojis", new JsonContent(guildEmojiProperties), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonEmoji>(), guildId, this);

    public async Task<GuildEmoji> ModifyGuildEmojiAsync(Snowflake guildId, Snowflake emojiId, Action<GuildEmojiOptions> action, RequestProperties? properties = null)
    {
        GuildEmojiOptions guildEmojiOptions = new();
        action(guildEmojiOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.ModifyGuildEmoji, guildId), new JsonContent(guildEmojiOptions), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonEmoji>(), guildId, this);
    }

    public Task DeleteGuildEmojiAsync(Snowflake guildId, Snowflake emojiId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/emojis/{emojiId}", new Route(RouteParameter.DeleteGuildEmoji, guildId), properties);
}