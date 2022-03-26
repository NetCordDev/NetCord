using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public async Task<Channel> GetChannelAsync(DiscordId channelId, RequestProperties? options = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", options).ConfigureAwait(false))!;
        return Channel.CreateFromJson(json.ToObject<JsonModels.JsonChannel>(), this);
    }

    public async Task<Channel> ModifyChannelAsync(DiscordId channelId, Action<GroupDMChannelOptions> action, RequestProperties? options = null)
    {
        GroupDMChannelOptions groupDMChannelOptions = new();
        action(groupDMChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(groupDMChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), this);
    }

    public async Task<Channel> DeleteChannelAsync(DiscordId channelId, RequestProperties? options = null)
        => Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public Task TriggerTypingStateAsync(DiscordId channelId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", options);

    public async Task<IDisposable> EnterTypingStateAsync(DiscordId channelId, RequestProperties? options = null)
    {
        TypingReminder typingReminder = new(channelId, this, options);
        await typingReminder.StartAsync().ConfigureAwait(false);
        return typingReminder;
    }

    public async Task<IReadOnlyDictionary<DiscordId, RestMessage>> GetPinnedMessagesAsync(DiscordId channelId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage[]>().ToDictionary(m => m.Id, m => new RestMessage(m, this));

    public Task PinMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", options);

    public Task UnpinMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", options);

    public Task GroupDMChannelAddUserAsync(DiscordId channelId, DiscordId userId, GroupDMUserAddProperties properties, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent(properties), $"/channels/{channelId}/recipients/{userId}", options);

    public Task GroupDMChannelDeleteUserAsync(DiscordId channelId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", options);

    /// <summary>
    /// Sends a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <returns></returns>
    public async Task<RestMessage> SendMessageAsync(DiscordId channelId, MessageProperties message, RequestProperties? options = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Post, message.Build(), $"/channels/{channelId}/messages", options).ConfigureAwait(false))!;
        return new(json.ToObject<JsonModels.JsonMessage>(), this);
    }

    public async Task<RestMessage> CrosspostMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);

    public async Task<RestMessage> ModifyMessageAsync(DiscordId channelId, DiscordId messageId, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, obj.Build(), $"/channels/{channelId}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonMessage>(), this);
    }

    public Task DeleteMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", options);

    public Task DeleteMessagesAsync(DiscordId channelId, IEnumerable<DiscordId> messagesIds, RequestProperties? options = null)
    {
        var ids = new DiscordId[100];
        int c = 0;
        List<Task> tasks = new();
        foreach (var id in messagesIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, options));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], options));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], options));
        return Task.WhenAll(tasks);
    }

    public async Task DeleteMessagesAsync(DiscordId channelId, IAsyncEnumerable<DiscordId> messagesIds, RequestProperties? options = null)
    {
        var ids = new DiscordId[100];
        int c = 0;
        List<Task> tasks = new();
        await foreach (var id in messagesIds)
        {
            ids[c] = id;
            if (c == 99)
            {
                tasks.Add(BulkDeleteMessagesAsync(channelId, ids, options));
                c = 0;
            }
            else
                c++;
        }
        if (c > 1)
            tasks.Add(BulkDeleteMessagesAsync(channelId, ids[..c], options));
        else if (c == 1)
            tasks.Add(DeleteMessageAsync(channelId, ids[0], options));
        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private Task BulkDeleteMessagesAsync(DiscordId channelId, DiscordId[] messagesIds, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"messages\":{JsonSerializer.Serialize(messagesIds, ToObjectExtensions._options)}}}"), $"/channels/{channelId}/messages/bulk-delete", options);

    public async Task<RestMessage> GetMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    => new((await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);

    public async IAsyncEnumerable<RestMessage> GetMessagesAsync(DiscordId channelId, RequestProperties? options = null)
    {
        byte messagesCount = 0;
        RestMessage? lastMessage = null;

        foreach (var message in await GetMaxMessagesAsyncTask(channelId, options).ConfigureAwait(false))
        {
            yield return lastMessage = message;
            messagesCount++;
        }
        if (messagesCount == 100)
        {
            await foreach (var message in GetMessagesBeforeAsync(channelId, lastMessage!, options))
                yield return message;
        }
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesBeforeAsyncTask(channelId, messageId, options).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    public async IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    {
        byte messagesCount;
        do
        {
            messagesCount = 0;
            foreach (var message in await GetMaxMessagesAfterAsyncTask(channelId, messageId, options).ConfigureAwait(false))
            {
                yield return message;
                messageId = message.Id;
                messagesCount++;
            }
        }
        while (messagesCount == 100);
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAsyncTask(DiscordId channelId, RequestProperties? options = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100", options).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAroundAsyncTask(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&around={messageId}", options).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesBeforeAsyncTask(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&before={messageId}", options).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }

    private async Task<IEnumerable<RestMessage>> GetMaxMessagesAfterAsyncTask(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
    {
        var messagsJson = (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/messages?limit=100&after={messageId}", options).ConfigureAwait(false))!;
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this)).Reverse();
    }

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