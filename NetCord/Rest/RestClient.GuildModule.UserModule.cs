namespace NetCord;

public partial class RestClient
{
    public partial class GuildModule
    {
        public class UserModule
        {
            private readonly RestClient _client;

            internal UserModule(RestClient client)
            {
                _client = client;
            }

            public async Task<GuildUser> GetAsync(DiscordId guildId, DiscordId userId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser>(), guildId, _client);

            public async IAsyncEnumerable<GuildUser> GetAsync(DiscordId guildId, RequestOptions? options = null)
            {
                short count = 0;
                GuildUser? lastUser = null;

                foreach (var user in await GetMaxUsersAsyncTask(guildId).ConfigureAwait(false))
                {
                    yield return lastUser = user;
                    count++;
                }
                while (count == 1000)
                {
                    count = 0;
                    foreach (var user in await GetMaxUsersAfterAsyncTask(guildId, lastUser!, options).ConfigureAwait(false))
                    {
                        yield return lastUser = user;
                        count++;
                    }
                }
            }

            private async Task<IEnumerable<GuildUser>> GetMaxUsersAsyncTask(DiscordId guildId, RequestOptions? options = null)
            {
                return (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000", options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(u => new GuildUser(u.ToObject<JsonModels.JsonGuildUser>(), guildId, _client));
            }

            private async Task<IEnumerable<GuildUser>> GetMaxUsersAfterAsyncTask(DiscordId guildId, DiscordId after, RequestOptions? options = null)
            {
                return (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000&after={after}", options).ConfigureAwait(false))!.RootElement.EnumerateArray().Select(u => new GuildUser(u.ToObject<JsonModels.JsonGuildUser>(), guildId, _client));
            }

            public async Task<IReadOnlyDictionary<DiscordId, GuildUser>> FindAsync(DiscordId guildId, string name, int limit, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search?query={Uri.EscapeDataString(name)}&limit={limit}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser[]>().ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, _client));

            public async Task<GuildUser?> AddAsync(DiscordId guildId, DiscordId userId, UserProperties userProperties, RequestOptions? options = null)
            {
                var v = await _client.SendRequestAsync(HttpMethod.Put, new JsonContent(userProperties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false);
                if (v == null)
                    return null;
                else
                    return new(v.ToObject<JsonModels.JsonGuildUser>(), guildId, _client);
            }

            public async Task<GuildUser> ModifyAsync(DiscordId guildId, DiscordId userId, Action<GuildUserProperties> action, RequestOptions? options = null)
            {
                GuildUserProperties properties = new();
                action(properties);
                var result = (await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!;
                return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, _client);
            }

            public async Task<GuildUser> ModifyCurrentAsync(DiscordId guildId, Action<CurrentGuildUserProperties> action, RequestOptions? options = null)
            {
                CurrentGuildUserProperties properties = new();
                action(properties);
                var result = (await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/@me", options).ConfigureAwait(false))!;
                return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, _client);
            }

            public Task AddRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

            public Task RemoveRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

            public Task KickAsync(DiscordId guildId, DiscordId userId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", options);

            public Task BanAsync(DiscordId guildId, DiscordId userId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", options);

            public Task BanAsync(DiscordId guildId, DiscordId userId, int deleteMessageDays, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Put, new JsonContent($"{{\"delete_message_days\":{deleteMessageDays}}}"), $"/guilds/{guildId}/bans/{userId}", options);

            public Task UnbanAsync(DiscordId guildId, DiscordId userId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", options);
        }
    }
}