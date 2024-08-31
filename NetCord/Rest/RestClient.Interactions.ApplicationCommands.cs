using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> GetGlobalApplicationCommandsAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));

    public async Task<ApplicationCommand> CreateGlobalApplicationCommandAsync(ulong applicationId, ApplicationCommandProperties applicationCommandProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, Serialization.Default.ApplicationCommandProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/applications/{applicationId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(ApplicationCommand)], nameof(ApplicationCommand.ApplicationId), nameof(ApplicationCommand.Id), Modifiers = ["virtual"], TypeNameOverride = $"Global{nameof(ApplicationCommand)}")]
    public async Task<ApplicationCommand> GetGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);

    [GenerateAlias([typeof(ApplicationCommand)], nameof(ApplicationCommand.ApplicationId), nameof(ApplicationCommand.Id), Modifiers = ["virtual"], TypeNameOverride = $"Global{nameof(ApplicationCommand)}")]
    public async Task<ApplicationCommand> ModifyGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, Action<ApplicationCommandOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        using (HttpContent content = new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, Serialization.Default.ApplicationCommandOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/{applicationId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(ApplicationCommand)], nameof(ApplicationCommand.ApplicationId), nameof(ApplicationCommand.Id), Modifiers = ["virtual"], TypeNameOverride = $"Global{nameof(ApplicationCommand)}")]
    public Task DeleteGlobalApplicationCommandAsync(ulong applicationId, ulong commandId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken);

    public async Task<IReadOnlyDictionary<ulong, ApplicationCommand>> BulkOverwriteGlobalApplicationCommandsAsync(ulong applicationId, IEnumerable<ApplicationCommandProperties> commands, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, Serialization.Default.IEnumerableApplicationCommandProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, content, $"/applications/{applicationId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new ApplicationCommand(c, this));
    }

    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> GetGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));

    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildApplicationCommand> CreateGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ApplicationCommandProperties applicationCommandProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandProperties>(applicationCommandProperties, Serialization.Default.ApplicationCommandProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/applications/{applicationId}/guilds/{guildId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(GuildApplicationCommand)], nameof(GuildApplicationCommand.ApplicationId), nameof(GuildApplicationCommand.GuildId), nameof(GuildApplicationCommand.Id), Modifiers = ["override"], CastType = typeof(ApplicationCommand))]
    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildApplicationCommand> GetGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);

    [GenerateAlias([typeof(GuildApplicationCommand)], nameof(GuildApplicationCommand.ApplicationId), nameof(GuildApplicationCommand.GuildId), nameof(GuildApplicationCommand.Id), Modifiers = ["override"], CastType = typeof(ApplicationCommand))]
    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildApplicationCommand> ModifyGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, Action<ApplicationCommandOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        ApplicationCommandOptions applicationCommandOptions = new();
        action(applicationCommandOptions);
        using (HttpContent content = new JsonContent<ApplicationCommandOptions>(applicationCommandOptions, Serialization.Default.ApplicationCommandOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommand).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(GuildApplicationCommand)], nameof(GuildApplicationCommand.ApplicationId), nameof(GuildApplicationCommand.GuildId), nameof(GuildApplicationCommand.Id), Modifiers = ["override"])]
    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task DeleteGuildApplicationCommandAsync(ulong applicationId, ulong guildId, ulong commandId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}", null, null, properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildApplicationCommand>> BulkOverwriteGuildApplicationCommandsAsync(ulong applicationId, ulong guildId, IEnumerable<ApplicationCommandProperties> commands, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ApplicationCommandProperties>>(commands, Serialization.Default.IEnumerableApplicationCommandProperties))
            return (await (await SendRequestAsync(HttpMethod.Put, content, $"/applications/{applicationId}/guilds/{guildId}/commands", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new GuildApplicationCommand(c, this));
    }

    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, ApplicationCommandGuildPermissions>> GetApplicationCommandsGuildPermissionsAsync(ulong applicationId, ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/permissions", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandGuildPermissionsArray).ConfigureAwait(false)).ToDictionary(p => p.CommandId, p => new ApplicationCommandGuildPermissions(p));

    [GenerateAlias([typeof(ApplicationCommand)], nameof(ApplicationCommand.ApplicationId), null, nameof(ApplicationCommand.Id))]
    [GenerateAlias([typeof(GuildApplicationCommand)], nameof(GuildApplicationCommand.ApplicationId), nameof(GuildApplicationCommand.GuildId), nameof(GuildApplicationCommand.Id), TypeNameOverride = "ApplicationCommandGuild")]
    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<ApplicationCommandGuildPermissions> GetApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandGuildPermissions).ConfigureAwait(false));

    [GenerateAlias([typeof(ApplicationCommand)], nameof(ApplicationCommand.ApplicationId), null, nameof(ApplicationCommand.Id))]
    [GenerateAlias([typeof(GuildApplicationCommand)], nameof(GuildApplicationCommand.ApplicationId), nameof(GuildApplicationCommand.GuildId), nameof(GuildApplicationCommand.Id), TypeNameOverride = "ApplicationCommandGuild")]
    [GenerateAlias([typeof(RestGuild)], null, nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<ApplicationCommandGuildPermissions> OverwriteApplicationCommandGuildPermissionsAsync(ulong applicationId, ulong guildId, ulong commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<ApplicationCommandGuildPermissionsProperties>(new(newPermissions), Serialization.Default.ApplicationCommandGuildPermissionsProperties))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationCommandGuildPermissions).ConfigureAwait(false));
    }
}
