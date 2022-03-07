using System.Text.Json.Serialization;

namespace NetCord;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> GetGlobalApplicationCommandAsync(DiscordId applicationId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandsAsync(DiscordId applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(DiscordId applicationId, DiscordId commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(DiscordId applicationId, DiscordId commandId, Action<ApplicationCommandOptions> action, RequestProperties? options = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGlobalApplicationCommandAsync(DiscordId applicationId, DiscordId commandId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", options);

    public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(DiscordId applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> GetGuildApplicationCommandsAsync(DiscordId applicationId, DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGuildApplicationCommandAsync(DiscordId applicationId, DiscordId guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGuildApplicationCommandAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGuildApplicationCommandAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, Action<ApplicationCommandOptions> action, RequestProperties? options = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGuildApplicationCommandAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options);

    public async Task<IReadOnlyDictionary<DiscordId, ApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(DiscordId applicationId, DiscordId guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<DiscordId, GuildApplicationCommandPermissions>> GetGuildApplicationCommandsPermissionsAsync(DiscordId applicationId, DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));

    public async Task<GuildApplicationCommandPermissions> GetGuildApplicationCommandPermissionsAsync(DiscordId applicationId, DiscordId guildId, DiscordId commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

    public async Task<GuildApplicationCommandPermissions> OverwriteGuildApplicationCommandPermissions(DiscordId applicationId, DiscordId guildId, DiscordId commandId, IEnumerable<ApplicationCommandPermissionProperties> newPermissions, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Put, new JsonContent(new Permissions(newPermissions)), $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

    private class Permissions
    {
        [JsonPropertyName("permissions")]
        public IEnumerable<ApplicationCommandPermissionProperties> NewPermissions { get; }

        public Permissions(IEnumerable<ApplicationCommandPermissionProperties> newPermissions)
        {
            NewPermissions = newPermissions;
        }
    }

    public async Task<IReadOnlyDictionary<DiscordId, GuildApplicationCommandPermissions>> BulkOverwriteGuildApplicationCommandPermissions(DiscordId applicationId, DiscordId guildId, IEnumerable<GuildApplicationCommandPermissionsProperties> newPermissions, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(newPermissions), $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));
}