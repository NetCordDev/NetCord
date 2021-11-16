using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public static class ChannelHelper
{
    public static async Task<Channel> GetChannelAsync(BotClient client, DiscordId channelId)
    {
        JsonDocument json = await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}", client).ConfigureAwait(false);
        return Channel.CreateFromJson(json.ToObject<JsonChannel>(), client);
    }

    #region GetMessages
    private const byte max = 100;

    public static async Task<Message> GetMessageAsync(BotClient client, DiscordId channelId, DiscordId messageId)
        => new((await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", client).ConfigureAwait(false)).ToObject<JsonMessage>(), client);

    public static async IAsyncEnumerable<Message> GetMessagesAsync(BotClient client, DiscordId channelId)
    {
        var messages = await GetMaxMessagesAsyncTask(client, channelId).ConfigureAwait(false);
        byte messagesCount = 0;
        Message lastMessage = null;

        foreach (var message in messages)
        {
            yield return lastMessage = message;
            messagesCount++;
        }
        if (messagesCount == max)
        {
            await foreach (var message in GetMessagesBeforeAsync(client, channelId, lastMessage))
                yield return message;
        }
    }

    public static async IAsyncEnumerable<Message> GetMessagesBeforeAsync(BotClient client, DiscordId channelId, DiscordId messageId)
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

    public static async IAsyncEnumerable<Message> GetMessagesAfterAsync(BotClient client, DiscordId channelId, DiscordId messageId)
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

    private static async Task<IEnumerable<Message>> GetMaxMessagesAsyncTask(BotClient client, DiscordId channelId)
    {
        var messagsJson = await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", client).ConfigureAwait(false);
        return messagsJson.RootElement.EnumerateArray().Select(m => new Message(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<Message>> GetMaxMessagesAroundAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", client).ConfigureAwait(false);
        return messagsJson.RootElement.EnumerateArray().Select(m => new Message(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<Message>> GetMaxMessagesBeforeAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", client).ConfigureAwait(false);
        return messagsJson.RootElement.EnumerateArray().Select(m => new Message(m.ToObject<JsonMessage>(), client));
    }

    private static async Task<IEnumerable<Message>> GetMaxMessagesAfterAsyncTask(BotClient client, DiscordId channelId, DiscordId messageId)
    {
        var messagsJson = await CDN.SendAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", client).ConfigureAwait(false);
        return messagsJson.RootElement.EnumerateArray().Select(m => new Message(m.ToObject<JsonMessage>(), client));
    }

    #endregion
    #region SendMessage

    /// <summary>
    /// Send a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <param name="message"></param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static async Task<Message> SendMessageAsync(BotClient client, BuiltMessage message, DiscordId channelId)
    {
        JsonDocument json = await CDN.SendAsync(HttpMethod.Post, message._content, $"/channels/{channelId}/messages", client).ConfigureAwait(false);
        return new(json.ToObject<JsonMessage>(), client);
    }

    /// <summary>
    /// Send a message to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <param name="content">a message content</param>
    /// <param name="channelId"></param>
    /// <returns></returns>
    public static Task<Message> SendMessageAsync(BotClient client, string content, DiscordId channelId)
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
            var jsonUsers = (await CDN.SendAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", client).ConfigureAwait(false)).ToObject<JsonModels.JsonThreadUser[]>();
            return jsonUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u));
        }
    }
}