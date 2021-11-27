using System.Text.Json;

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

    public static async Task DeleteAsync(BotClient client, DiscordId channelId, IEnumerable<DiscordId> messagesIds)
    {
        var ids = new DiscordId[100];
        int c = 0;
        List<Task> tasks = new();
        foreach (var id in messagesIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteAsync(client, channelId, ids));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteAsync(client, channelId, ids[..c]));
        else if (c == 1)
            tasks.Add(DeleteAsync(client, channelId, ids[0]));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static Task BulkDeleteAsync(BotClient client, DiscordId channelId, DiscordId[] messagesIds)
        => CDN.SendAsync(HttpMethod.Post, $"{{\"messages\":{JsonSerializer.Serialize(messagesIds)}}}", $"/channels/{channelId}/messages/bulk-delete", client);
}