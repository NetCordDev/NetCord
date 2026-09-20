using System.Text;

using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    /// <summary>
    /// Creates a new guild.
    /// </summary>
    /// <param name="guildProperties">The properties of the guild to create.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The created guild.</returns>
    /// <remarks>
    /// Discord deprecated application-driven guild creation in April 2025 and removed the corresponding API endpoint in July 2025.
    /// </remarks>
    [Obsolete("Discord deprecated application-driven guild creation in April 2025 and removed the corresponding API endpoint in July 2025.")]
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildProperties>(guildProperties, Serialization.Default.GuildProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Gets a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get.</param>
    /// <param name="withCounts">Whether to include the member counts in the response.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The retrieved guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<RestGuild> GetGuildAsync(ulong guildId, bool withCounts = false, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}", $"?with_counts={withCounts}", new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    /// <summary>
    /// Gets a guild preview.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get a preview for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The retrieved guild preview.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildPreview> GetGuildPreviewAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify.</param>
    /// <param name="action">The action to perform on the guild options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<RestGuild> ModifyGuildAsync(ulong guildId, Action<GuildOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildOptions guildOptions = new();
        action(guildOptions);
        using (HttpContent content = new JsonContent<GuildOptions>(guildOptions, Serialization.Default.GuildOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuild).ConfigureAwait(false), this);
    }

    /// <summary>
    /// Deletes a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to delete.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task DeleteGuildAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets the channels of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get channels for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<IGuildChannel>> GetGuildChannelsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannelArray).ConfigureAwait(false)).Select(c => IGuildChannel.CreateFromJson(c, guildId, this)).ToArray();

    /// <summary>
    /// Creates a new channel in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to create the channel in.</param>
    /// <param name="channelProperties">The properties of the new channel.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The created channel.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IGuildChannel> CreateGuildChannelAsync(ulong guildId, GuildChannelProperties channelProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildChannelProperties>(channelProperties, Serialization.Default.GuildChannelProperties))
            return IGuildChannel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonChannel).ConfigureAwait(false), guildId, this);
    }

    /// <summary>
    /// Modifies the positions of channels in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify channel positions in.</param>
    /// <param name="positions">The new positions of the channels.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task ModifyGuildChannelPositionsAsync(ulong guildId, IEnumerable<GuildChannelPositionProperties> positions, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<GuildChannelPositionProperties>>(positions, Serialization.Default.IEnumerableGuildChannelPositionProperties))
            await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/channels", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the active threads in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get active threads for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the active threads in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<GuildThread>> GetActiveGuildThreadsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => GuildThreadGenerator.CreateThreads(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildThreadResult).ConfigureAwait(false), this).ToArray();

    /// <summary>
    /// Gets a guild member.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the member for.</param>
    /// <param name="userId">The ID of the user to get.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The guild member.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id), Modifiers = ["new"])]
    public async Task<GuildUser> GetGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);

    /// <summary>
    /// Gets the users in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the users for.</param>
    /// <param name="paginationProperties">The properties for pagination.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <returns>An async enumerable of the guild users.</returns>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    /// <summary>
    /// Searches for a user in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to search in.</param>
    /// <param name="name">The name of the user to search for.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the users that match the search query.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<GuildUser>> FindGuildUserAsync(ulong guildId, string name, int limit, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search", $"?query={Uri.EscapeDataString(name)}&limit={limit}", new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this)).ToArray();

    /// <summary>
    /// Adds a user to a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to add the user to.</param>
    /// <param name="userId">The ID of the user to add.</param>
    /// <param name="userProperties">The properties of the user to add.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The added guild user, or null if the user could not be added.</returns>
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

    /// <summary>
    /// Modifies a user in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the user in.</param>
    /// <param name="userId">The ID of the user to modify.</param>
    /// <param name="action">The action to perform on the user options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified guild user.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public async Task<GuildUser> ModifyGuildUserAsync(ulong guildId, ulong userId, Action<GuildUserOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildUserOptions guildUserOptions = new();
        action(guildUserOptions);
        using (HttpContent content = new JsonContent<GuildUserOptions>(guildUserOptions, Serialization.Default.GuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    /// <summary>
    /// Modifies the current user in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the current user in.</param>
    /// <param name="action">The action to perform on the current user options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified guild user.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildUser> ModifyCurrentGuildUserAsync(ulong guildId, Action<CurrentGuildUserOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        CurrentGuildUserOptions currentGuildUserOptions = new();
        action(currentGuildUserOptions);
        using (HttpContent content = new JsonContent<CurrentGuildUserOptions>(currentGuildUserOptions, Serialization.Default.CurrentGuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/members/@me", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    /// <summary>
    /// Adds a role to a user in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to add the role to.</param>
    /// <param name="userId">The ID of the user to add the role to.</param>
    /// <param name="roleId">The ID of the role to add.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task AddGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes a role from a user in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to remove the role from.</param>
    /// <param name="userId">The ID of the user to remove the role from.</param>
    /// <param name="roleId">The ID of the role to remove.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task RemoveGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Removes a user from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to remove the user from.</param>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task KickGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets the bans in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the bans for.</param>
    /// <param name="paginationProperties">The properties for pagination.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <returns>An async enumerable of the guild bans.</returns>
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
            new(paginationProperties.BatchSize.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(guildId),
            properties);
    }

    /// <summary>
    /// Gets a ban in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the ban for.</param>
    /// <param name="userId">The ID of the user to get the ban for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The guild ban.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildBan> GetGuildBanAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildBan).ConfigureAwait(false), guildId, this);

    /// <summary>
    /// Bans a user from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to ban the user from.</param>
    /// <param name="userId">The ID of the user to ban.</param>
    /// <param name="deleteMessageSeconds">The number of seconds to delete messages for. Must be between 0 and 604800 (7 days).</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public async Task BanGuildUserAsync(ulong guildId, ulong userId, int deleteMessageSeconds = 0, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildBanProperties>(new(deleteMessageSeconds), Serialization.Default.GuildBanProperties))
            await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Bans multiple users from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to ban the users from.</param>
    /// <param name="userIds">The IDs of the users to ban. Up to 200 users can be banned at once.</param>
    /// <param name="deleteMessageSeconds">The number of seconds to delete messages for. Must be between 0 and 604800 (7 days).</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The result of the bulk ban operation, including the IDs of the banned users and the user IDs of failed bans.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildBulkBan> BanGuildUsersAsync(ulong guildId, IEnumerable<ulong> userIds, int deleteMessageSeconds = 0, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildBulkBanProperties>(new(userIds, deleteMessageSeconds), Serialization.Default.GuildBulkBanProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/bulk-ban", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildBulkBan).ConfigureAwait(false));
    }

    /// <summary>
    /// Unbans a user from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to unban the user from.</param>
    /// <param name="userId">The ID of the user to unban.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(GuildBan)], nameof(GuildBan.GuildId), $"{nameof(GuildBan.User)}.{nameof(GuildBan.User.Id)}", NameOverride = "DeleteAsync", ClientName = "client")]
    [GenerateAlias([typeof(GuildUser)], nameof(GuildUser.GuildId), nameof(GuildUser.Id))]
    public Task UnbanGuildUserAsync(ulong guildId, ulong userId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets the roles of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the roles for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the roles in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Role>> GetGuildRolesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).Select(r => new Role(r, guildId, this)).ToArray();

    /// <summary>
    /// Gets a role in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the role for.</param>
    /// <param name="roleId">The ID of the role to get.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The role in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<Role> GetGuildRoleAsync(ulong guildId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);

    /// <summary>
    /// Creates a new role in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to create the role in.</param>
    /// <param name="guildRoleProperties">The properties of the new role.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The created role.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<Role> CreateGuildRoleAsync(ulong guildId, RoleProperties guildRoleProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<RoleProperties>(guildRoleProperties, Serialization.Default.RoleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    /// <summary>
    /// Modifies the positions of roles in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the role positions in.</param>
    /// <param name="positions">The new positions of the roles.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified roles.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Role>> ModifyGuildRolePositionsAsync(ulong guildId, IEnumerable<RolePositionProperties> positions, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<IEnumerable<RolePositionProperties>>(positions, Serialization.Default.IEnumerableRolePositionProperties))
            return (await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRoleArray).ConfigureAwait(false)).Select(r => new Role(r, guildId, this)).ToArray();
    }

    /// <summary>
    /// Modifies a role in a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the role in.</param>
    /// <param name="roleId">The ID of the role to modify.</param>
    /// <param name="action">The action to perform on the role options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified role.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(Role)], nameof(Role.GuildId), nameof(Role.Id), TypeNameOverride = $"{nameof(Guild)}{nameof(Role)}")]
    public async Task<Role> ModifyGuildRoleAsync(ulong guildId, ulong roleId, Action<RoleOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        RoleOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<RoleOptions>(obj, Serialization.Default.RoleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRole).ConfigureAwait(false), guildId, this);
    }

    /// <summary>
    /// Deletes a role from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to delete the role from.</param>
    /// <param name="roleId">The ID of the role to delete.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias([typeof(Role)], nameof(Role.GuildId), nameof(Role.Id), TypeNameOverride = $"{nameof(Guild)}{nameof(Role)}")]
    public Task DeleteGuildRoleAsync(ulong guildId, ulong roleId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Modifies the MFA level of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the MFA level for.</param>
    /// <param name="mfaLevel">The new MFA level of the guild.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The new MFA level of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<MfaLevel> ModifyGuildMfaLevelAsync(ulong guildId, MfaLevel mfaLevel, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildMfaLevelProperties>(new GuildMfaLevelProperties(mfaLevel), Serialization.Default.GuildMfaLevelProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/mfa", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildMfaLevel).ConfigureAwait(false)).Level;
    }

    /// <summary>
    /// Gets the number of members that would be pruned from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the prune count for.</param>
    /// <param name="days">The number of days to consider for pruning. Must be between 1 and 30. Default is 7.</param>
    /// <param name="roles">The roles to consider for pruning.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The number of members that would be pruned.</returns>
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

    /// <summary>
    /// Prunes members from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to prune members from.</param>
    /// <param name="pruneProperties">The properties of the prune operation.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The number of members that were pruned.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<int?> GuildPruneAsync(ulong guildId, GuildPruneProperties pruneProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<GuildPruneProperties>(pruneProperties, Serialization.Default.GuildPruneProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/prune", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildPruneResult).ConfigureAwait(false)).Pruned;
    }

    /// <summary>
    /// Gets the voice regions of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the voice regions for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the voice regions in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));

    /// <summary>
    /// Gets the invites of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the invites for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the invites in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IEnumerable<RestInvite>> GetGuildInvitesAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInviteArray).ConfigureAwait(false)).Select(i => new RestInvite(i, this));

    /// <summary>
    /// Gets the integrations of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the integrations for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A list of the integrations in the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyList<Integration>> GetGuildIntegrationsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonIntegrationArray).ConfigureAwait(false)).Select(i => new Integration(i, this)).ToArray();

    /// <summary>
    /// Deletes an integration from a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to delete the integration from.</param>
    /// <param name="integrationId">The ID of the integration to delete.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public Task DeleteGuildIntegrationAsync(ulong guildId, ulong integrationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", null, new(guildId), properties, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets the widget settings of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the widget settings for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The widget settings of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));

    /// <summary>
    /// Modifies the widget settings of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the widget settings for.</param>
    /// <param name="action">The action to perform on the widget settings options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified widget settings of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(ulong guildId, Action<GuildWidgetSettingsOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        using (HttpContent content = new JsonContent<GuildWidgetSettingsOptions>(guildWidgetSettingsOptions, Serialization.Default.GuildWidgetSettingsOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/widget", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidgetSettings).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the widget of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the widget for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The widget of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWidget> GetGuildWidgetAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWidget).ConfigureAwait(false), this);

    /// <summary>
    /// Gets the vanity invite of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the vanity invite for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The vanity invite of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildVanityInvite).ConfigureAwait(false));

    /// <summary>
    /// Gets the welcome screen of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the welcome screen for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The welcome screen of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));

    /// <summary>
    /// Modifies the welcome screen of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the welcome screen for.</param>
    /// <param name="action">The action to perform on the welcome screen options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified welcome screen of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(ulong guildId, Action<GuildWelcomeScreenOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        using (HttpContent content = new JsonContent<GuildWelcomeScreenOptions>(guildWelcomeScreenOptions, Serialization.Default.GuildWelcomeScreenOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/welcome-screen", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildWelcomeScreen).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets the onboarding of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to get the onboarding for.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The onboarding of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildOnboarding> GetGuildOnboardingAsync(ulong guildId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/onboarding", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);

    /// <summary>
    /// Modifies the onboarding of a guild.
    /// </summary>
    /// <param name="guildId">The ID of the guild to modify the onboarding for.</param>
    /// <param name="action">The action to perform on the onboarding options.</param>
    /// <param name="properties">The properties of the request.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>The modified onboarding of the guild.</returns>
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildOnboarding> ModifyGuildOnboardingAsync(ulong guildId, Action<GuildOnboardingOptions> action, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        GuildOnboardingOptions guildOnboardingOptions = new();
        action(guildOnboardingOptions);
        using (HttpContent content = new JsonContent<GuildOnboardingOptions>(guildOnboardingOptions, Serialization.Default.GuildOnboardingOptions))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/guilds/{guildId}/onboarding", null, new(guildId), properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildOnboarding).ConfigureAwait(false), this);
    }
}
