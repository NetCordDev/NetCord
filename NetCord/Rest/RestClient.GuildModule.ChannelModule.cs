namespace NetCord;

public partial class RestClient
{
    public partial class GuildModule
    {
        public class ChannelModule
        {
            private readonly RestClient _client;

            internal ChannelModule(RestClient client)
            {
                _client = client;
            }

            public async Task<Channel> CreateAsync(DiscordId guildId, GuildChannelProperties channelBuilder, RequestOptions? options = null)
                => NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(channelBuilder), $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);

            public async Task<Channel> ModifyAsync(DiscordId channelId, Action<GuildChannelOptions> action, RequestOptions? options = null)
            {
                GuildChannelOptions guildChannelOptions = new();
                action(guildChannelOptions);
                return NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(guildChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);
            }

            public async Task<Channel> ModifyAsync(DiscordId channelId, Action<ThreadOptions> action, RequestOptions? options = null)
            {
                ThreadOptions threadOptions = new();
                action(threadOptions);
                return NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(threadOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);
            }

            public Task ModifyPositionsAsync(DiscordId guildId, ChannelPosition[] positions, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/channels", options);

            public async Task<(IReadOnlyDictionary<DiscordId, Thread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetActiveThreadsAsync(DiscordId guildId, RequestOptions? options = null)
            {
                var json = (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", options).ConfigureAwait(false))!.RootElement;
                var threads = json.GetProperty("threads").ToObject<JsonModels.JsonChannel[]>();
                var users = json.GetProperty("members").ToObject<JsonModels.JsonThreadUser[]>();
                return (threads.ToDictionary(t => t.Id, t => (Thread)NetCord.Channel.CreateFromJson(t, _client)), users.ToDictionary(u => u.ThreadId, u => new ThreadUser(u)));
            }
        }
    }
}