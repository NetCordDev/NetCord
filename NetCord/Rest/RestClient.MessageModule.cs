using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public partial class MessageModule
    {
        private readonly RestClient _client;

        internal MessageModule(RestClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Sends a <paramref name="message"/> to a specified channel by <paramref name="channelId"/>
        /// </summary>
        /// <returns></returns>
        public async Task<RestMessage> SendAsync(DiscordId channelId, MessageProperties message, RequestOptions? options = null)
        {
            JsonDocument json = (await _client.SendRequestAsync(HttpMethod.Post, message.Build(), $"/channels/{channelId}/messages", options).ConfigureAwait(false))!;
            return new(json.ToObject<JsonMessage>(), _client);
        }

        public async Task<RestMessage> CrosspostAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => new((await _client.SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/messages/{messageId}/crosspost", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), _client);

        public async Task<RestMessage> ModifyAsync(DiscordId channelId, DiscordId messageId, Action<MessageOptions> action, RequestOptions? options = null)
        {
            MessageOptions obj = new();
            action(obj);
            return new((await _client.SendRequestAsync(HttpMethod.Patch, obj.Build(), $"/channels/{channelId}/messages/{messageId}", options).ConfigureAwait(false))!.ToObject<JsonMessage>(), _client);
        }

        public Task DeleteAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/messages/{messageId}", options);

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
            => _client.SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"messages\":{JsonSerializer.Serialize(messagesIds)}}}"), $"/channels/{channelId}/messages/bulk-delete", options);
    }
}