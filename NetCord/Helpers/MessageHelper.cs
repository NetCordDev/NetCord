namespace NetCord;

public static class MessageHelper
{
    public static Task AddReactionAsync(BotClient client, ReactionEmoji emoji, DiscordId channelId, DiscordId messageId)
        => CDN.SendAsync(HttpMethod.Put, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}/@me", client);

    public static Task DeleteReactionAsync(BotClient client, ReactionEmoji emoji, DiscordId userId, DiscordId channelId, DiscordId messageId)
        => CDN.SendAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}/{userId}", client);

    public static Task DeleteAllReactionsAsync(BotClient client, ReactionEmoji emoji, DiscordId channelId, DiscordId messageId)
        => CDN.SendAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions/{(emoji.EmojiType == ReactionEmoji.Type.Standard ? emoji.Name : emoji.Name + ":" + emoji.Id)}", client);

    public static Task DeleteAllReactionsAsync(BotClient client, DiscordId channelId, DiscordId messageId)
        => CDN.SendAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}/reactions", client);

    public static Task DeleteAsync(BotClient client, DiscordId channelId, DiscordId messageId)
        => CDN.SendAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", client);

    public static Task DeleteAsync(BotClient client, DiscordId channelId, DiscordId messageId, string reason)
        => CDN.SendAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", client, reason);
}