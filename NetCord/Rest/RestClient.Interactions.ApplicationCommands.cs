namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> GetGlobalApplicationCommandsAsync(ulong applicationId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(ulong applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);
    }

    public Task DeleteGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(ulong applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> GetGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    public async Task<GuildApplicationCommand> CreateGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<GuildApplicationCommand> GetGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<GuildApplicationCommand> ModifyGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);
    }

    public Task DeleteGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    public async Task<IReadOnlyDictionary<ulong, ApplicationCommandGuildPermissions>> GetApplicationCommandsGuildPermissionsAsync(ulong applicationId, ulong guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsArraySerializerContext.WithOptions.JsonApplicationCommandGuildPermissionsArray).ToDictionary(p => p.CommandId, p => new ApplicationCommandGuildPermissions(p));

    public async Task<ApplicationCommandGuildPermissions> GetApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions));

    public async Task<ApplicationCommandGuildPermissions> OverwriteApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", new(RateLimits.RouteParameter.OverwriteApplicationCommandGuildPermissions), new JsonContent<ApplicationCommandGuildPermissionsProperties>(new(newPermissions), ApplicationCommandGuildPermissionsProperties.ApplicationCommandGuildPermissionsPropertiesSerializerContext.WithOptions.ApplicationCommandGuildPermissionsProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions));
}
