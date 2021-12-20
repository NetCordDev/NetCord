using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public partial class MessageModule
    {
        private readonly BotClient _client;

        internal MessageModule(BotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Send a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task<RestMessage> SendAsync(BuiltMessage message, DiscordId channelId, RequestOptions? options = null)
        {
            JsonDocument json = (await _client.Rest.SendRequestAsync(HttpMethod.Post, message._content, $"/channels/{channelId}/messages", options).ConfigureAwait(false))!;
            return new(json.ToObject<JsonMessage>(), _client);
        }

        /// <summary>
        /// Send a message to a specified channel by <paramref name="channelId"/>
        /// </summary>
        /// <param name="content">a message content</param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public Task<RestMessage> SendAsync(string content, DiscordId channelId, RequestOptions? options = null)
        {
            MessageBuilder messageBuilder = new()
            {
                Content = content
            };
            return SendAsync(messageBuilder.Build(), channelId, options);
        }

        public Task DeleteAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.Rest.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", options);

        public Task DeleteAsync(DiscordId channelId, IEnumerable<DiscordId> messagesIds, RequestOptions? options = null)
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
                tasks.Add(DeleteAsync(channelId, ids[0], options));
            return Task.WhenAll(tasks);
        }

        public async Task DeleteAsync(DiscordId channelId, IAsyncEnumerable<DiscordId> messagesIds, RequestOptions? options = null)
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
                tasks.Add(DeleteAsync(channelId, ids[0], options));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private Task BulkDeleteMessagesAsync(DiscordId channelId, DiscordId[] messagesIds, RequestOptions? options = null)
            => _client.Rest.SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"messages\":{JsonSerializer.Serialize(messagesIds)}}}"), $"/channels/{channelId}/messages/bulk-delete", options);
    }
}