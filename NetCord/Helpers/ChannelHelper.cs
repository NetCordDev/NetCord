using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public static class ChannelHelper
{
    public static async Task<Channel> GetChannelAsync(BotClient client, DiscordId channelId)
    {
        JsonDocument json = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}", client).ConfigureAwait(false))!;
        return Channel.CreateFromJson(json.ToObject<JsonChannel>(), client);
    }

    #region GetMessages
    private const byte max = 100;

    public static async Task<RestMessage> GetMessageAsync(BotClient client, DiscordId channelId, DiscordId messageId)
        => new(((await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", client).ConfigureAwait(false))!).ToObject<JsonMessage>(), client);

    public static async IAsyncEnumerable<RestMessage> GetMessagesAsync(BotClient client, DiscordId channelId)
    {
        var messages = await GetMaxMessagesAsyncTask(client, channelId).ConfigureAwait(false);
        byte messagesCount = 0;
        RestMessage? lastMessage = null;

        foreach (var message in messages)
        {
            yield return lastMessage = message;
            messagesCount++;
        }
        if (messagesCount == max)
        {
            await foreach (var message in GetMessagesBeforeAsync(client, channelId, lastMessage!))
                yield return message;
        }
    }

    public static async IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        byte messagesCount;
        do
        {
            var messages = await GetMaxMessagesBeforeAsyncTask(client, channelId, messageId).ConfigureAwait(false);
            messagesCount = 0;
            foreach (var message in messages)
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == max);
    }

    public static async IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        byte messagesCount;
        do
        {
            var messages = await GetMaxMessagesAfterAsyncTask(client, channelId, messageId).ConfigureAwait(false);
            messagesCount = 0;
            foreach (var message in messages)
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == max);
    }

    private static async Task<IEnumerable<RestMessage>> GetMaxMessagesAsyncTask(BotClient client, DiscordId channelId)
    {
        var messagsJson = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", client).ConfigureAwait(false))!;
        return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<RestMessage>> GetMaxMessagesAroundAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", client).ConfigureAwait(false))!;
        return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<RestMessage>> GetMaxMessagesBeforeAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", client).ConfigureAwait(false))!;
        return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<RestMessage>> GetMaxMessagesAfterAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", client).ConfigureAwait(false))!;
        return messagsJson.RootElement.EnumerateArray().Select(m => new RestMessage(m.ToObject<JsonMessage>(), client));
    }

    #endregion
    #region SendMessage

    /// <summary>
    /// Send a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static async Task<RestMessage> SendMessageAsync(BotClient client, BuiltMessage message, DiscordId channelId)
    {
        JsonDocument json = await CDN.SendAsync(HttpMethod.Post, message, $"/channels/{channelId}/messages", client).ConfigureAwait(false);
        return new(json.ToObject<JsonMessage>(), client);
    }

    /// <summary>
    /// Send a message to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <param name="content">a message content</param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static Task<RestMessage> SendMessageAsync(BotClient client, string content, DiscordId channelId)
    {
        MessageBuilder messageBuilder = new()
        {
            Content = content
        };
        return SendMessageAsync(client, messageBuilder.Build(), channelId);
    }

    #endregion

    public static class Thread
    {
        public static async Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetUsersAsync(BotClient client, DiscordId threadId)
        {
            var jsonUsers = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", client).ConfigureAwait(false))!.ToObject<JsonThreadUser[]>();
            return jsonUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u));
        }
    }
}