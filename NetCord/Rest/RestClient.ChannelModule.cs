using System.Text.Json;

using NetCord.JsonModels;

namespace NetCord;

public partial class RestClient
{
    public class ChannelModule
    {
        private readonly RestClient _client;

        internal ChannelModule(RestClient client)
        {
            _client = client;
        }

        public async Task<Channel> GetAsync(DiscordId channelId, RequestOptions? options = null)
        {
            JsonDocument json = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}", options).ConfigureAwait(false))!;
            return NetCord.Channel.CreateFromJson(json.ToObject<JsonChannel>(), _client);
        }

        public async Task<Channel> ModifyAsync(DiscordId channelId, Action<GroupDMChannelOptions> action, RequestOptions? options = null)
        {
            GroupDMChannelOptions groupDMChannelOptions = new();
            action(groupDMChannelOptions);
            return NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(groupDMChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), _client);
        }

        public async Task<Channel> DeleteAsync(DiscordId channelId, RequestOptions? options = null)
            => NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonChannel>(), _client);

        public async Task<DMChannel> GetDMByUserIdAsync(DiscordId userId, RequestOptions? options = null)
            => new DMChannel(((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"recipient_id\":\"{userId}\"}}"), "/users/@me/channels", options).ConfigureAwait(false))!).ToObject<JsonChannel>(), _client);

        public Task TriggerTypingStateAsync(DiscordId channelId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/typing", options);

        public async Task<IDisposable> EnterTypingStateAsync(DiscordId channelId, RequestOptions? options = null)
        {
            TypingReminder typingReminder = new(channelId, _client, options);
            await typingReminder.StartAsync().ConfigureAwait(false);
            return typingReminder;
        }

        public async Task<IReadOnlyDictionary<DiscordId, RestMessage>> GetPinnedMessagesAsync(DiscordId channelId, RequestOptions? options = null)
            => (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/pins", options).ConfigureAwait(false))!.ToObject<JsonMessage[]>().ToDictionary(m => m.Id, m => new RestMessage(m, _client));

        public Task PinAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Put, $"/channels/{channelId}/pins/{messageId}", options);

        public Task UnpinAsync(DiscordId channelId, DiscordId messageId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/pins/{messageId}", options);

        public Task GroupDMAddUser(DiscordId channelId, DiscordId userId, GroupDMUserAddProperties properties, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Put, new JsonContent(properties), $"/channels/{channelId}/recipients/{userId}", options);

        public Task GroupDMDeleteUser(DiscordId channelId, DiscordId userId, RequestOptions? options = null)
            => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/recipients/{userId}", options);
    }
}