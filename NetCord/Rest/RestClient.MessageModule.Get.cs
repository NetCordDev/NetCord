using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
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
        return messagsJson.ToObject<IEnumerable<JsonMessage>>().Select(m => new RestMessage(m, this));
    }
}