namespace NetCord;

public partial class RestClient
{
    public partial class MessageModule
    {
        public Task AddReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/@me", options);

        public Task DeleteReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, DiscordId userId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}/{userId}", options);

        public async IAsyncEnumerable<User> GetReactionAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestOptions? options = null)
        {
            var partialUrl = $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}?limit=100";
            short count = 0;
            User? lastUser = null;

            foreach (var user in (await _client.SendRequestAsync(HttpMethod.Get, partialUrl, options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(u => new User(u.ToObject<JsonModels.JsonUser>(), _client)))
            {
                yield return lastUser = user;
                count++;
            }
            partialUrl += $"&after={lastUser}";
            while (count == 1000)
            {
                count = 0;
                foreach (var user in (await _client.SendRequestAsync(HttpMethod.Get, partialUrl, options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(u => new User(u.ToObject<JsonModels.JsonUser>(), _client)))
                {
                    yield return lastUser = user;
                    count++;
                }
            }
        }

        public Task DeleteAllReactionsAsync(DiscordId channelId, DiscordId messageId, ReactionEmojiProperties emoji, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{ReactionEmojiToString(emoji)}", options);

        public Task DeleteAllReactionsAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", options);

        private static string ReactionEmojiToString(ReactionEmojiProperties emoji) => emoji.EmojiType == ReactionEmojiProperties.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id;
    }
}