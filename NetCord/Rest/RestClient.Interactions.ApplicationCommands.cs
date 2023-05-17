using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> GetGlobalApplicationCommandsAsync(ulong applicationId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(ulong applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        using (HttpContent content = new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    public Task DeleteGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(ulong applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));
    }

    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> GetGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    public async Task<GuildApplicationCommand> CreateGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    public async Task<GuildApplicationCommand> GetGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);

    public async Task<GuildApplicationCommand> ModifyGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        using (HttpContent content = new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    public Task DeleteGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));
    }

    public async Task<IReadOnlyDictionary<ulong, ApplicationCommandGuildPermissions>> GetApplicationCommandsGuildPermissionsAsync(ulong applicationId, ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsArraySerializerContext.WithOptions.JsonApplicationCommandGuildPermissionsArray).ConfigureAwait(false)).ToDictionary(p => p.CommandId, p => new ApplicationCommandGuildPermissions(p));

    public async Task<ApplicationCommandGuildPermissions> GetApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions).ConfigureAwait(false));

    public async Task<ApplicationCommandGuildPermissions> OverwriteApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandGuildPermissionsProperties>(new(newPermissions), ApplicationCommandGuildPermissionsProperties.ApplicationCommandGuildPermissionsPropertiesSerializerContext.WithOptions.ApplicationCommandGuildPermissionsProperties))
            return new(await (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", new(RateLimits.RouteParameter.OverwriteApplicationCommandGuildPermissions), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions).ConfigureAwait(false));
    }
}
