using System.Text.Json.Serialization;

namespace NetCord;

public partial class RestClient
{
    public partial class InteractionModule
    {
        public class ApplicationCommandModule
        {
            private readonly RestClient _client;

            internal ApplicationCommandModule(RestClient client)
            {
                _client = client;
            }

            public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> GetGlobalAsync(DiscordId applicationId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

            public async Task<ApplicationCommand> CreateGlobalAsync(DiscordId applicationId, ApplicationCommandProperties applicationCommandProperties, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

            public async Task<ApplicationCommand> GetGlobalAsync(DiscordId applicationId, DiscordId commandId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

            public async Task<ApplicationCommand> ModifyGlobalAsync(DiscordId applicationId, DiscordId commandId, Action<ApplicationCommandOptions> action, RequestOptions? options = null)
            {
                ApplicationCommandOptions applicationCommandOptions = new();
                action(applicationCommandOptions);
                return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
            }

            public Task DeleteGlobalAsync(DiscordId applicationId, DiscordId commandId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", options);

            public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> BulkOverwriteGlobalAsync(DiscordId applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

            public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> GetGuildAsync(DiscordId applicationId, DiscordId guildId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

            public async Task<ApplicationCommand> CreateGuildAsync(DiscordId applicationId, ApplicationCommandProperties applicationCommandProperties, DiscordId guildId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

            public async Task<ApplicationCommand> GetGuildAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

            public async Task<ApplicationCommand> ModifyGuildAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, Action<ApplicationCommandOptions> action, RequestOptions? options = null)
            {
                ApplicationCommandOptions applicationCommandOptions = new();
                action(applicationCommandOptions);
                return new((await _client.SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
            }

            public Task DeleteGuildAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestOptions? options = null)
                => _client.SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options);

            public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> BulkOverwriteGuildAsync(DiscordId applicationId, DiscordId guildId, IEnumerable<ApplicationCommandProperties> commands, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

            public async Task<IReadOnlyDictionary<DiscordId, GuildApplicationCommandPermissions>> GetGuildPermissionsAsync(DiscordId applicationId, DiscordId guildId, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));

            public async Task<GuildApplicationCommandPermissions> GetGuildPermissionsAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

            public async Task<GuildApplicationCommandPermissions> OverwriteApplicationCommandPermissions(DiscordId applicationId, DiscordId guildId, DiscordId commandId, IEnumerable<ApplicationCommandPermissionProperties> newPermissions, RequestOptions? options = null)
                => new((await _client.SendRequestAsync(HttpMethod.Put, new JsonContent(new Permissions(newPermissions)), $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

            private class Permissions
            {
                [JsonPropertyName("permissions")]
                public IEnumerable<ApplicationCommandPermissionProperties> NewPermissions { get; }

                public Permissions(IEnumerable<ApplicationCommandPermissionProperties> newPermissions)
                {
                    NewPermissions = newPermissions;
                }
            }

            public async Task<IReadOnlyDictionary<DiscordId, GuildApplicationCommandPermissions>> BulkOverwriteApplicationCommandPermissions(DiscordId applicationId, DiscordId guildId, IEnumerable<GuildApplicationCommandPermissionsProperties> newPermissions, RequestOptions? options = null)
                => (await _client.SendRequestAsync(HttpMethod.Put, new JsonContent(newPermissions), $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));
        }
    }
}