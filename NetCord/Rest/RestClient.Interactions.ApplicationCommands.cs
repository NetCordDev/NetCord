using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> GetGlobalApplicationCommandsAsync(Snowflake applicationId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(Snowflake applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent(applicationCommandProperties), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent(applicationCommandOptions), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(Snowflake applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent(commands), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> GetGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<ApplicationCommand> CreateGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent(applicationCommandProperties), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> GetGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());

    public async Task<ApplicationCommand> ModifyGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent(applicationCommandOptions), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand>());
    }

    public Task DeleteGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent(commands), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonApplicationCommand[]>().ToDictionary(c => c.Id, c => new ApplicationCommand(c));

    public async Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommandPermissions>> GetGuildApplicationCommandsPermissionsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions[]>().ToDictionary(p => p.CommandId, p => new GuildApplicationCommandPermissions(p));

    public async Task<GuildApplicationCommandPermissions> GetGuildApplicationCommandPermissionsAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

    public async Task<GuildApplicationCommandPermissions> OverwriteGuildApplicationCommandPermissionsAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, IEnumerable<ApplicationCommandPermissionProperties> newPermissions, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", new(RateLimits.RouteParameter.OverwriteGuildApplicationCommandPermissions), new JsonContent(new Permissions(newPermissions)), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildApplicationCommandPermissions>());

    private class Permissions
    {
        [JsonPropertyName("permissions")]
        public IEnumerable<ApplicationCommandPermissionProperties> NewPermissions { get; }

        public Permissions(IEnumerable<ApplicationCommandPermissionProperties> newPermissions)
        {
            NewPermissions = newPermissions;
        }
    }
}