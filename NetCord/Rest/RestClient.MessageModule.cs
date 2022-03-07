using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    /// <summary>
    /// Sends a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
    /// </summary>
    /// <returns></returns>
    public async Task<RestMessage> SendMessageAsync(DiscordId channelId, MessageProperties message, RequestProperties? options = null)
    {
        var json = (await SendRequestAsync(HttpMethod.Post, message.Build(), $"/channels/{channelId}/messages", options).ConfigureAwait(false))!;
        return new(json.ToObject<JsonMessage>(), this);
    }

    public async Task<RestMessage> CrosspostMessageAsync(DiscordId channelId, DiscordId messageId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);

    public async Task<RestMessage> ModifyMessageAsync(DiscordId channelId, DiscordId messageId, Action<MessageOptions> action, RequestProperties? options = null)
    {
        MessageOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, obj.Build(), $"/channels/{channelId}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), this);
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
        => SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"messages\":{JsonSerializer.Serialize(messagesIds)}}}"), $"/channels/{channelId}/messages/bulk-delete", options);
}