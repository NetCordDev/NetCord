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

            public async Task<IReadOnlyDictionary<DiscordId, IGuildChannel>> GetAsync(DiscordId guildId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel[]>().ToDictionary(c => c.Id, c => (IGuildChannel)NetCord.Channel.CreateFromJson(c, _client));

            public async Task<IGuildChannel> CreateAsync(DiscordId guildId, GuildChannelProperties channelBuilder, RequestOptions? options = null)
                => (IGuildChannel)NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(channelBuilder), $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);

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

            public Task ModifyPermissionsAsync(DiscordId channelId, ChannelPermissionOverwrite permissionOverwrite, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, new JsonContent(permissionOverwrite), $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", options);

            public async Task<IEnumerable<GuildInvite>> GetInvitesAsync(DiscordId channelId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(r => new GuildInvite(r.ToObject<JsonModels.JsonGuildInvite>(), _client));

            public async Task<GuildInvite> CreateInviteAsync(DiscordId channelId, GuildInviteProperties? guildInviteProperties = null, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(guildInviteProperties), $"/channels/{channelId}/invites", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildInvite>(), _client);

            public Task DeletePermissionAsync(DiscordId channelId, DiscordId overwriteId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", options);

            public async Task<FollowedChannel> FollowNewsChannelAsync(DiscordId channelId, DiscordId targetChannelId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(@$"{{""webhook_channel_id"":{targetChannelId}}}"), $"/channels/{channelId}/followers", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonFollowedChannel>());

            public async Task<Thread> CreateThreadAsync(DiscordId channelId, DiscordId messageId, ThreadWithMessageProperties properties, RequestOptions? options = null)
                => (Thread)NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/channels/{channelId}/messages/{messageId}/threads", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);

            public async Task<Thread> CreateThreadAsync(DiscordId channelId, ThreadProperties properties, RequestOptions? options = null)
                => (Thread)NetCord.Channel.CreateFromJson((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/channels/{channelId}/threads", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), _client);

            public Task JoinThreadAsync(DiscordId threadId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", options);

            public Task AddThreadUser(DiscordId threadId, DiscordId userId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", options);

            public Task LeaveThreadAsync(DiscordId threadId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", options);

            public Task DeleteThreadUser(DiscordId threadId, DiscordId userId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", options);

            public async Task<ThreadUser> GetThreadUserAsync(DiscordId threadId, DiscordId userId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonThreadUser>(), _client);

            public async Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetThreadUsersAsync(DiscordId threadId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonThreadUser[]>().ToDictionary(u => u.UserId, u => new ThreadUser(u, _client));

            public async Task<(IReadOnlyDictionary<DiscordId, Thread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetActiveThreadsAsync(DiscordId guildId, RequestOptions? options = null)
            {
                var json = (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", options).ConfigureAwait(false))!.RootElement;
                var threads = json.GetProperty("threads").ToObject<JsonModels.JsonChannel[]>();
                var users = json.GetProperty("members").ToObject<JsonModels.JsonThreadUser[]>();
                return (threads.ToDictionary(t => t.Id, t => (Thread)NetCord.Channel.CreateFromJson(t, _client)), users.ToDictionary(u => u.ThreadId, u => new ThreadUser(u, _client)));
            }

            public async IAsyncEnumerable<Thread> GetPublicArchivedThreadsAsync(DiscordId channelId, RequestOptions? options = null)
            {
                string url = $"/channels/{channelId}/threads/archived/public?limit=100";
                var json = (await _client.SendRequestAsync(HttpMethod.Get, url, options).ConfigureAwait(false))!.RootElement;
                var threads = json.GetProperty("threads");
                var users = json.GetProperty("members");
                Thread? last = null;
                foreach (var t in threads.EnumerateArray().Zip(users.EnumerateArray()))
                    yield return last = (Thread)NetCord.Channel.CreateFromJson(t.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = t.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, _client);
                await foreach (var v in GetPublicArchivedThreadsAsync(channelId, last!.Metadata.ArchiveTimestamp, options))
                    yield return v;
            }

            public async IAsyncEnumerable<Thread> GetPublicArchivedThreadsAsync(DiscordId channelId, DateTimeOffset before, RequestOptions? options = null)
            {
                string url = $"/channels/{channelId}/threads/archived/public?limit=100&before=";
                var json = (await _client.SendRequestAsync(HttpMethod.Get, $"{url}{before:s}", options).ConfigureAwait(false))!.RootElement;
                var threads = json.GetProperty("threads");
                var users = json.GetProperty("members");
                Thread? last = null;
                foreach (var v in threads.EnumerateArray().Zip(users.EnumerateArray()))
                    yield return last = (Thread)NetCord.Channel.CreateFromJson(v.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = v.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, _client);
                while (json.GetProperty("has_more").GetBoolean())
                {
                    json = (await _client.SendRequestAsync(HttpMethod.Get, $"{url}{last!.Metadata.ArchiveTimestamp:s}", options).ConfigureAwait(false))!.RootElement;
                    threads = json.GetProperty("threads");
                    users = json.GetProperty("members");
                    foreach (var v in threads.EnumerateArray().Zip(users.EnumerateArray()))
                        yield return last = (Thread)NetCord.Channel.CreateFromJson(v.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = v.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, _client);
                }
            }

            public async Task<(IReadOnlyDictionary<DiscordId, Thread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetPrivateArchivedThreadsAsync(DiscordId channelId, RequestOptions? options = null)
            {
                var json = (await _client.SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private", options).ConfigureAwait(false))!.RootElement;
                var threads = json.GetProperty("threads").ToObject<JsonModels.JsonChannel[]>();
                var users = json.GetProperty("members").ToObject<JsonModels.JsonThreadUser[]>();
                return (threads.ToDictionary(t => t.Id, t => (Thread)NetCord.Channel.CreateFromJson(t, _client)), users.ToDictionary(u => u.ThreadId, u => new ThreadUser(u, _client)));
            }
        }
    }
}