namespace NetCord;

public partial class RestClient
{
    public Task AddMessageReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", options);

    public Task DeleteMessageReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", options);

    public async IAsyncEnumerable<User> GetMessageReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestProperties? options = null)
    {
        var partialUrl = $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100";
        short count = 0;
        User? lastUser = null;

        foreach (var user in (await SendRequestAsync(HttpMethod.Get, partialUrl, options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonUser>>().Select(u => new User(u, this)))
        {
            yield return lastUser = user;
            count++;
        }
        partialUrl += $"&after={lastUser}";
        while (count == 1000)
        {
            count = 0;
            foreach (var user in (await SendRequestAsync(HttpMethod.Get, partialUrl, options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonUser>>().Select(u => new User(u, this)))
            {
                yield return lastUser = user;
                count++;
            }
        }
    }

    public Task DeleteAllMessageReactionsAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", options);

    public Task DeleteAllMessageReactionsAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", options);

    private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.EmojiType == ReactionEmojiProperties.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id;
}