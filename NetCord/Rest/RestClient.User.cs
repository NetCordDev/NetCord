using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(CurrentUser)], Modifiers = ["new"])]
    public async Task<CurrentUser> GetCurrentUserAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonUser).ConfigureAwait(false), this);

    [GenerateAlias([typeof(User)], nameof(User.Id))]
    public async Task<User> GetUserAsync(ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonUser).ConfigureAwait(false), this);

    [GenerateAlias([typeof(CurrentUser)])]
    public async Task<CurrentUser> ModifyCurrentUserAsync(Action<CurrentUserOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentUserOptions currentUserOptions = new();
        action(currentUserOptions);
        using (HttpContent content = new JsonContent<CurrentUserOptions>(currentUserOptions, Serialization.Default.CurrentUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/users/@me", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonUser).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(CurrentUser)])]
    public IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(GuildsPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.After, 200);

        return new QueryPaginationAsyncEnumerable<RestGuild, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildArray).ConfigureAwait(false)).Select(g => new RestGuild(g, this)),
            g => g.Id,
            HttpMethod.Get,
            $"/users/@me/guilds",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), $"?with_counts={paginationProperties.WithCounts}&"),
            null,
            properties);
    }

    [GenerateAlias([typeof(CurrentUser)])]
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id))]
    public async Task<GuildUser> GetCurrentUserGuildUserAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(CurrentUser)])]
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task LeaveGuildAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", null, null, properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(User)], nameof(User.Id))]
    public async Task<DMChannel> GetDMChannelAsync(ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<DMChannelProperties>(new(userId), Serialization.Default.DMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/users/@me/channels", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties groupDMChannelProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GroupDMChannelProperties>(groupDMChannelProperties, Serialization.Default.GroupDMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/users/@me/channels", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(CurrentUser)])]
    public async Task<IReadOnlyList<Connection>> GetCurrentUserConnectionsAsync(RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/connections", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonConnectionArray).ConfigureAwait(false)).Select(c => new Connection(c, this)).ToArray();

    [GenerateAlias([typeof(CurrentUser)])]
    public async Task<ApplicationRoleConnection> GetCurrentUserApplicationRoleConnectionAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/applications/{applicationId}/role-connection", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationRoleConnection).ConfigureAwait(false));

    [GenerateAlias([typeof(CurrentUser)])]
    public async Task<ApplicationRoleConnection> UpdateCurrentUserApplicationRoleConnectionAsync(ulong applicationId, ApplicationRoleConnectionProperties applicationRoleConnectionProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<ApplicationRoleConnectionProperties>(applicationRoleConnectionProperties, Serialization.Default.ApplicationRoleConnectionProperties))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/users/@me/applications/{applicationId}/role-connection", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonApplicationRoleConnection).ConfigureAwait(false));
    }
}
