using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
    public partial class GuildModule
    {
        public class UserModule
        {
            private readonly BotClient _client;

            internal UserModule(BotClient client)
            {
                _client = client;
            }

            public async Task<GuildUser> ModifyAsync(DiscordId guildId, DiscordId userId, Action<GuildUserProperties> func, RequestOptions? options = null)
            {
                GuildUserProperties properties = new();
                func.Invoke(properties);
                var result = (await _client.Rest.SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!;
                return new(result.ToObject<JsonModels.JsonGuildUser>(), _client.Guilds[guildId], _client);
            }

            public Task AddRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

            public Task RemoveRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

            public Task KickAsync(DiscordId userId, DiscordId guildId, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", options);

            public Task BanAsync(DiscordId userId, DiscordId guildId, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", options);

            public Task BanAsync(DiscordId userId, DiscordId guildId, int deleteMessageDays, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Put, new JsonContent($"{{\"delete_message_days\":{deleteMessageDays}}}"), $"/guilds/{guildId}/bans/{userId}", options);

            public Task UnbanAsync(DiscordId userId, DiscordId guildId, RequestOptions? options = null)
                => _client.Rest.SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", options);

            public string GetGuildAvatarUrl(DiscordId guildId, DiscordId userId, string? guildAvatarHash, ImageFormat? format)
            {
                if (guildAvatarHash != null)
                    return $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}";
                else
                    throw new InvalidOperationException("This user has no guild avatar");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="guildId"></param>
            /// <param name="userId"></param>
            /// <param name="guildAvatarHash"></param>
            /// <param name="format"></param>
            /// <param name="size">any power of two between 16 and 4096</param>
            /// <returns></returns>
            public string GetGuildAvatarUrl(DiscordId guildId, DiscordId userId, string guildAvatarHash, ImageFormat? format, int size)
            {
                if (guildAvatarHash != null)
                    return $"{Discord.ImageBaseUrl}/guilds/{guildId}/users/{userId}/avatars/{guildAvatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : guildAvatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}";
                else
                    throw new InvalidOperationException("This user has no guild avatar");
            }
        }
    }
}