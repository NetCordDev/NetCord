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

        public async Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetThreadUsersAsync(DiscordId threadId, RequestOptions? options = null)
        {
            var jsonUsers = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", options).ConfigureAwait(false))!.ToObject<JsonThreadUser[]>();
            return jsonUsers.ToDictionary(u => u.UserId, u => new ThreadUser(u));
        }
    }
}