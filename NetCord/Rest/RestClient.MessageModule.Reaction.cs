namespace NetCord;

public partial class RestClient
{
    public partial class MessageModule
    {
        public Task AddReactionAsync(ReactionEmoji emoji, DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}/@me", options);

        public Task DeleteReactionAsync(ReactionEmoji emoji, DiscordId userId, DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}/{userId}", options);

        public Task DeleteAllReactionsAsync(ReactionEmoji emoji, DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}", options);

        public Task DeleteAllReactionsAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", options);
    }
}