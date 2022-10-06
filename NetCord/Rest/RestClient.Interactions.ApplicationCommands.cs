namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> GetGlobalApplicationCommandsAsync(Snowflake applicationId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(Snowflake applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);
    }

    public Task DeleteGlobalApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(Snowflake applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommand>> GetGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    public async Task<GuildApplicationCommand> CreateGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.CreateApplicationCommand), new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, ApplicationCommandProperties.ApplicationCommandPropertiesSerializerContext.WithOptions.ApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<GuildApplicationCommand> GetGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);

    public async Task<GuildApplicationCommand> ModifyGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new(RateLimits.RouteParameter.ModifyApplicationCommand), new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, ApplicationCommandOptions.ApplicationCommandOptionsSerializerContext.WithOptions.ApplicationCommandOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandSerializerContext.WithOptions.JsonApplicationCommand), this);
    }

    public Task DeleteGuildApplicationCommandAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteApplicationCommand), properties);

    public async Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(Snowflake applicationId, Snowflake guildId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands", new(RateLimits.RouteParameter.BulkOverwriteApplicationCommands), new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, ApplicationCommandProperties.IEnumerableOfApplicationCommandPropertiesSerializerContext.WithOptions.IEnumerableApplicationCommandProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommand.JsonApplicationCommandArraySerializerContext.WithOptions.JsonApplicationCommandArray).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    public async Task<IReadOnlyDictionary<Snowflake, ApplicationCommandGuildPermissions>> GetApplicationCommandsGuildPermissionsAsync(Snowflake applicationId, Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsArraySerializerContext.WithOptions.JsonApplicationCommandGuildPermissionsArray).ToDictionary(p => p.CommandId, p => new ApplicationCommandGuildPermissions(p));

    public async Task<ApplicationCommandGuildPermissions> GetApplicationCommandGuildPermissionsAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions));

    public async Task<ApplicationCommandGuildPermissions> OverwriteApplicationCommandGuildPermissionsAsync(Snowflake applicationId, Snowflake guildId, Snowflake commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Put, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", new(RateLimits.RouteParameter.OverwriteApplicationCommandGuildPermissions), new JsonContent<ApplicationCommandGuildPermissionsProperties>(new(newPermissions), ApplicationCommandGuildPermissionsProperties.ApplicationCommandGuildPermissionsPropertiesSerializerContext.WithOptions.ApplicationCommandGuildPermissionsProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonApplicationCommandGuildPermissions.JsonApplicationCommandGuildPermissionsSerializerContext.WithOptions.JsonApplicationCommandGuildPermissions));
}
