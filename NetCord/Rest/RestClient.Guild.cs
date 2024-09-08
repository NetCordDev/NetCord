using System.Text;

using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildProperties>(guildProperties, Serialization.Default.GuildProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<RestGuild> GetGuildAsync(ulong guildId, bool withCounts = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}", $"?with_counts={withCounts}", new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildPreview> GetGuildPreviewAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<RestGuild> ModifyGuildAsync(ulong guildId, Action<GuildOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildOptions guildOptions = new();
        action(guildOptions);
        using (HttpContent content = new JsonContent<GuildOptions>(guildOptions, Serialization.Default.GuildOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task DeleteGuildAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<IGuildChannel>> GetGuildChannelsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannelArray).ConfigureAwait(false)).Select(c => IGuildChannel.CreateFromJson(c, guildId, this)).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IGuildChannel> CreateGuildChannelAsync(ulong guildId, GuildChannelProperties channelProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildChannelProperties>(channelProperties, Serialization.Default.GuildChannelProperties))
            return IGuildChannel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task ModifyGuildChannelPositionsAsync(ulong guildId, IEnumerable<GuildChannelPositionProperties> positions, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<GuildChannelPositionProperties>>(positions, Serialization.Default.IEnumerableGuildChannelPositionProperties))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<GuildThread>> GetActiveGuildThreadsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => GuildThreadGenerator.CreateThreads(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildThreadResult).ConfigureAwait(false), this).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id), Modifiers = ["new"])]
    public async Task<GuildUser> GetGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<GuildUser> GetGuildUsersAsync(ulong guildId, PaginationProperties<ulong>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 1000);

        return new QueryPaginationAsyncEnumerable<GuildUser, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/members",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<GuildUser>> FindGuildUserAsync(ulong guildId, string name, int limit, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search", $"?query={Uri.EscapeDataString(name)}&limit={limit}", new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this)).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildUser?> AddGuildUserAsync(ulong guildId, ulong userId, GuildUserProperties userProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        Stream? stream;
        using (HttpContent content = new JsonContent<GuildUserProperties>(userProperties, Serialization.Default.GuildUserProperties))
            stream = await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (stream.Length == 0)
            return null;
        else
            return new(await stream.ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public async Task<GuildUser> ModifyGuildUserAsync(ulong guildId, ulong userId, Action<GuildUserOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildUserOptions guildUserOptions = new();
        action(guildUserOptions);
        using (HttpContent content = new JsonContent<GuildUserOptions>(guildUserOptions, Serialization.Default.GuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildUser> ModifyCurrentGuildUserAsync(ulong guildId, Action<CurrentGuildUserOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentGuildUserOptions currentGuildUserOptions = new();
        action(currentGuildUserOptions);
        using (HttpContent content = new JsonContent<CurrentGuildUserOptions>(currentGuildUserOptions, Serialization.Default.CurrentGuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/@me", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task AddGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task RemoveGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task KickGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<GuildBan> GetGuildBansAsync(ulong guildId, PaginationProperties<ulong>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.After, 1000);

        return new QueryPaginationAsyncEnumerable<GuildBan, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildBanArray).ConfigureAwait(false)).Select(b => new GuildBan(b, guildId, this)),
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildBanArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(b => new GuildBan(b, guildId, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            b => b.User.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/bans",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildBan> GetGuildBanAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildBan).ConfigureAwait(false), guildId, this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public async Task BanGuildUserAsync(ulong guildId, ulong userId, int deleteMessageSeconds = 0, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildBanProperties>(new(deleteMessageSeconds), Serialization.Default.GuildBanProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildBulkBan> BanGuildUsersAsync(ulong guildId, IEnumerable<ulong> userIds, int deleteMessageSeconds = 0, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildBulkBanProperties>(new(userIds, deleteMessageSeconds), Serialization.Default.GuildBulkBanProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/bulk-ban", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildBulkBan).ConfigureAwait(false));
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildBan)], nameof(GuildBan.GuildId), $"{nameof(GuildBan.User)}.{nameof(GuildBan.User.Id)}", NameOverride = "DeleteAsync", ClientName = "client")]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task UnbanGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Role>> GetGuildRolesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).Select(r => new Role(r, guildId, this)).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<Role> CreateGuildRoleAsync(ulong guildId, RoleProperties guildRoleProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<RoleProperties>(guildRoleProperties, Serialization.Default.RoleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Role>> ModifyGuildRolePositionsAsync(ulong guildId, IEnumerable<RolePositionProperties> positions, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<RolePositionProperties>>(positions, Serialization.Default.IEnumerableRolePositionProperties))
            return (await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).Select(r => new Role(r, guildId, this)).ToArray();
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(Role)], nameof(Role.Id), TypeNameOverride = $"{nameof(Guild)}{nameof(Role)}")]
    public async Task<Role> ModifyGuildRoleAsync(ulong guildId, ulong roleId, Action<RoleOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        RoleOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<RoleOptions>(obj, Serialization.Default.RoleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(Role)], nameof(Role.Id), TypeNameOverride = $"{nameof(Guild)}{nameof(Role)}")]
    public Task DeleteGuildRoleAsync(ulong guildId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<MfaLevel> ModifyGuildMfaLevelAsync(ulong guildId, MfaLevel mfaLevel, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildMfaLevelProperties>(new GuildMfaLevelProperties(mfaLevel), Serialization.Default.GuildMfaLevelProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/mfa", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildMfaLevel).ConfigureAwait(false)).Level;
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<int> GetGuildPruneCountAsync(ulong guildId, int days, IEnumerable<ulong>? roles = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        var query = roles is null
            ? $"?days={days}"
            : new StringBuilder()
                .Append("?days=")
                .Append(days)
                .Append("&include_roles=")
                .AppendJoin(',', roles)
                .ToString();
        return (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune", query, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildPruneCountResult).ConfigureAwait(false)).Pruned;
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<int?> GuildPruneAsync(ulong guildId, GuildPruneProperties pruneProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildPruneProperties>(pruneProperties, Serialization.Default.GuildPruneProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/prune", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildPruneResult).ConfigureAwait(false)).Pruned;
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<RestInvite>> GetGuildInvitesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInviteArray).ConfigureAwait(false)).Select(i => new RestInvite(i, this));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Integration>> GetGuildIntegrationsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonIntegrationArray).ConfigureAwait(false)).Select(i => new Integration(i, this)).ToArray();

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task DeleteGuildIntegrationAsync(ulong guildId, ulong integrationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(ulong guildId, Action<GuildWidgetSettingsOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        using (HttpContent content = new JsonContent<GuildWidgetSettingsOptions>(guildWidgetSettingsOptions, Serialization.Default.GuildWidgetSettingsOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/widget", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidget> GetGuildWidgetAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidget).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildVanityInvite).ConfigureAwait(false));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(ulong guildId, Action<GuildWelcomeScreenOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        using (HttpContent content = new JsonContent<GuildWelcomeScreenOptions>(guildWelcomeScreenOptions, Serialization.Default.GuildWelcomeScreenOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildOnboarding> GetGuildOnboardingAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/onboarding", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildOnboarding> ModifyGuildOnboardingAsync(ulong guildId, Action<GuildOnboardingOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildOnboardingOptions guildOnboardingOptions = new();
        action(guildOnboardingOptions);
        using (HttpContent content = new JsonContent<GuildOnboardingOptions>(guildOnboardingOptions, Serialization.Default.GuildOnboardingOptions))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/onboarding", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task ModifyCurrentGuildUserVoiceStateAsync(ulong guildId, Action<CurrentUserVoiceStateOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentUserVoiceStateOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<CurrentUserVoiceStateOptions>(obj, Serialization.Default.CurrentUserVoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/@me", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), null, nameof(GuildUser.Id))]
    public async Task ModifyGuildUserVoiceStateAsync(ulong guildId, ulong channelId, ulong userId, Action<VoiceStateOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        using (HttpContent content = new JsonContent<VoiceStateOptions>(obj, Serialization.Default.VoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/voice-states/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
