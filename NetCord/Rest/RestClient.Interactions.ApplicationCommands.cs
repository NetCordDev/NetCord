using System.Text.Json.Serialization;

namespace NetCord;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> GetGlobalApplicationCommandAsync(Snowflake applicationId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(Snowflake applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? options = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", options);

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(Snowflake applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> GetGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(applicationCommandProperties), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? options = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(applicationCommandOptions), $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", options);

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(commands), $"/applications/{applicationId}/guilds/{guildId}/commands", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommandPermissions>> GetGuildApplicationCommandsPermissionsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));

    public async Task<GuildApplicationCommandPermissions> GetGuildApplicationCommandPermissionsAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

    public async Task<GuildApplicationCommandPermissions> OverwriteGuildApplicationCommandPermissions(Snowflake applicationId, Snowflake guildId, Snowflake commandId, IEnumerable<ApplicationCommandPermissionProperties> newPermissions, RequestProperties? options = null)
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

    public async Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommandPermissions>> BulkOverwriteGuildApplicationCommandPermissions(Snowflake applicationId, Snowflake guildId, IEnumerable<GuildApplicationCommandPermissionsProperties> newPermissions, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Put, new JsonContent(newPermissions), $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));
}