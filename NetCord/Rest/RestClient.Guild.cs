namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildProperties>(guildProperties, GuildProperties.GuildPropertiesSerializerContext.WithOptions.GuildProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, "/guilds", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);
    }

    public async Task<RestGuild> GetGuildAsync(ulong guildId, bool withCounts = false, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);

    public async Task<RestGuild> ModifyGuildAsync(ulong guildId, Action<GuildOptions> action, RequestProperties? properties = null)
    {
        GuildOptions guildProperties = new();
        action(guildProperties);
        using (HttpContent content = new JsonContent<GuildOptions>(guildProperties, GuildOptions.GuildOptionsSerializerContext.WithOptions.GuildOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}", new(RateLimits.RouteParameter.Guilds), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild).ConfigureAwait(false), this);
    }

    public Task DeleteGuildAsync(ulong guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", properties);

    public async Task<IReadOnlyDictionary<ulong, IGuildChannel>> GetGuildChannelsAsync(ulong guildId, RequestProperties? properties = null)
    => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", new RateLimits.Route(RateLimits.RouteParameter.GetGuildChannels), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonChannel.JsonChannelArraySerializerContext.WithOptions.JsonChannelArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => (IGuildChannel)NetCord.Channel.CreateFromJson(c, this));

    public async Task<IGuildChannel> CreateGuildChannelAsync(ulong guildId, GuildChannelProperties channelProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildChannelProperties>(channelProperties, GuildChannelProperties.GuildChannelPropertiesSerializerContext.WithOptions.GuildChannelProperties))
            return (IGuildChannel)Channel.CreateFromJson(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/channels", new(RateLimits.RouteParameter.GuildChannels), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task ModifyGuildChannelPositionsAsync(ulong guildId, IEnumerable<ChannelPositionProperties> positions, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<ChannelPositionProperties>>(positions, ChannelPositionProperties.IEnumerableOfChannelPositionPropertiesSerializerContext.WithOptions.IEnumerableChannelPositionProperties))
            await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/channels", new(RateLimits.RouteParameter.GuildChannelPositions), content, properties).ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<ulong, GuildThread>> GetActiveGuildThreadsAsync(ulong guildId, RequestProperties? properties = null)
        => GuildThreadGenerator.CreateThreads(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRestGuildThreadResult.JsonRestGuildThreadResultSerializerContext.WithOptions.JsonRestGuildThreadResult).ConfigureAwait(false), this);

    public async Task<GuildUser> GetGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUser), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAsync(ulong guildId, RequestProperties? properties = null)
    {
        short count = 0;
        GuildUser? lastUser = null;

        foreach (var user in await GetMaxGuildUsersAsyncTask(guildId).ConfigureAwait(false))
        {
            yield return lastUser = user;
            count++;
        }
        if (count == 1000)
        {
            await foreach (var user in GetGuildUsersAfterAsync(guildId, lastUser!.Id, properties))
                yield return user;
        }
    }

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAfterAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
    {
        short count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildUsersAfterAsyncTask(guildId, userId, properties).ConfigureAwait(false))
            {
                yield return user;
                userId = user.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAsyncTask(ulong guildId, RequestProperties? properties = null)
    {
        return (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUsers), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this));
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAfterAsyncTask(ulong guildId, ulong after, RequestProperties? properties = null)
    {
        return (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000&after={after}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUsers), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).ConfigureAwait(false)).Select(u => new GuildUser(u, guildId, this));
    }

    public async Task<IReadOnlyDictionary<ulong, GuildUser>> FindGuildUserAsync(ulong guildId, string name, int limit, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search?query={Uri.EscapeDataString(name)}&limit={limit}", new RateLimits.Route(RateLimits.RouteParameter.FindGuildUser), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).ConfigureAwait(false)).ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, this));

    public async Task<GuildUser?> AddGuildUserAsync(ulong guildId, ulong userId, GuildUserProperties userProperties, RequestProperties? properties = null)
    {
        var v = await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}", new(RateLimits.RouteParameter.AddGuildUser), new JsonContent<GuildUserProperties>(userProperties, GuildUserProperties.GuildUserPropertiesSerializerContext.WithOptions.GuildUserProperties), properties).ConfigureAwait(false);
        if (v == null)
            return null;
        else
            return new(await v.ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildUser> ModifyGuildUserAsync(ulong guildId, ulong userId, Action<GuildUserOptions> action, RequestProperties? properties = null)
    {
        GuildUserOptions guildUserOptions = new();
        action(guildUserOptions);
        using (HttpContent content = new JsonContent<GuildUserOptions>(guildUserOptions, GuildUserOptions.GuildUserOptionsSerializerContext.WithOptions.GuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", new(RateLimits.RouteParameter.ModifyGuildUser), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public async Task<GuildUser> ModifyCurrentGuildUserAsync(ulong guildId, Action<CurrentGuildUserOptions> action, RequestProperties? properties = null)
    {
        CurrentGuildUserOptions currentGuildUserOptions = new();
        action(currentGuildUserOptions);
        using (HttpContent content = new JsonContent<CurrentGuildUserOptions>(currentGuildUserOptions, CurrentGuildUserOptions.CurrentGuildUserOptionsSerializerContext.WithOptions.CurrentGuildUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", new(RateLimits.RouteParameter.ModifyCurrentGuildUser), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);
    }

    public Task AddGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", new RateLimits.Route(RateLimits.RouteParameter.AddRemoveGuildUserRole), properties);

    public Task RemoveGuildUserRoleAsync(ulong guildId, ulong userId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", new RateLimits.Route(RateLimits.RouteParameter.AddRemoveGuildUserRole), properties);

    public Task KickGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", new RateLimits.Route(RateLimits.RouteParameter.KickGuildUser), properties);

    public async IAsyncEnumerable<GuildBan> GetGuildBansAsync(ulong guildId, RequestProperties? properties = null)
    {
        int count = 0;
        GuildBan? last = null;

        foreach (var ban in await GetMaxGuildBansAsyncTask(guildId, properties).ConfigureAwait(false))
        {
            yield return last = ban;
            count++;
        }
        if (count == 1000)
        {
            await foreach (var ban in GetGuildBansAfterAsync(guildId, last!.User.Id, properties))
                yield return ban;
        }
    }

    public async IAsyncEnumerable<GuildBan> GetGuildBansBeforeAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
    {
        int count;
        do
        {
            count = 0;
            foreach (var ban in await GetMaxGuildBansBeforeAsyncTask(guildId, userId, properties).ConfigureAwait(false))
            {
                yield return ban;
                userId = ban.User.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    public async IAsyncEnumerable<GuildBan> GetGuildBansAfterAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
    {
        int count;
        do
        {
            count = 0;
            foreach (var ban in await GetMaxGuildBansAfterAsyncTask(guildId, userId, properties).ConfigureAwait(false))
            {
                yield return ban;
                userId = ban.User.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAsyncTask(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).ConfigureAwait(false)).Select(b => new GuildBan(b, guildId, this));

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansBeforeAsyncTask(ulong guildId, ulong before, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?before={before}", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).ConfigureAwait(false)).Select(b => new GuildBan(b, guildId, this)).Reverse();

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAfterAsyncTask(ulong guildId, ulong after, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?after={after}", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).ConfigureAwait(false)).Select(b => new GuildBan(b, guildId, this));

    public async Task<GuildBan> GetGuildBanAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildBan.JsonGuildBanSerializerContext.WithOptions.JsonGuildBan).ConfigureAwait(false), guildId, this);

    public async Task BanGuildUserAsync(ulong guildId, ulong userId, int deleteMessageSeconds = 0, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildBanProperties>(new(deleteMessageSeconds), GuildBanProperties.GuildBanPropertiesSerializerContext.WithOptions.GuildBanProperties))
            await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", content, properties).ConfigureAwait(false);
    }

    public Task UnbanGuildUserAsync(ulong guildId, ulong userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", properties);

    public async Task<IReadOnlyDictionary<ulong, Role>> GetRolesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRole.JsonRoleArraySerializerContext.WithOptions.JsonRoleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new Role(r, guildId, this));

    public async Task<Role> CreateRoleAsync(ulong guildId, RoleProperties guildRoleProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<RoleProperties>(guildRoleProperties, RoleProperties.RolePropertiesSerializerContext.WithOptions.RoleProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/roles", new(RateLimits.RouteParameter.CreateRole), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRole.JsonRoleSerializerContext.WithOptions.JsonRole).ConfigureAwait(false), guildId, this);
    }

    public async Task<IReadOnlyDictionary<ulong, Role>> ModifyRolePositionsAsync(ulong guildId, IEnumerable<RolePositionProperties> positions, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<IEnumerable<RolePositionProperties>>(positions, RolePositionProperties.IEnumerableOfRolePositionPropertiesSerializerContext.WithOptions.IEnumerableRolePositionProperties))
            return (await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles", new(RateLimits.RouteParameter.ModifyRolePositions), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRole.JsonRoleArraySerializerContext.WithOptions.JsonRoleArray).ConfigureAwait(false)).ToDictionary(r => r.Id, r => new Role(r, guildId, this));
    }

    public async Task<Role> ModifyRoleAsync(ulong guildId, ulong roleId, Action<RoleOptions> action, RequestProperties? properties = null)
    {
        RoleOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<RoleOptions>(obj, RoleOptions.RoleOptionsSerializerContext.WithOptions.RoleOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", new(RateLimits.RouteParameter.ModifyRole), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRole.JsonRoleSerializerContext.WithOptions.JsonRole).ConfigureAwait(false), guildId, this);
    }

    public Task DeleteRoleAsync(ulong guildId, ulong roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", properties);

    public async Task<MfaLevel> ModifyGuildMfaLevelAsync(ulong guildId, MfaLevel mfaLevel, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildMfaLevelProperties>(new GuildMfaLevelProperties(mfaLevel), GuildMfaLevelProperties.GuildMfaLevelPropertiesSerializerContext.WithOptions.GuildMfaLevelProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/mfa", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildMfaLevel.JsonGuildMfaLevelSerializerContext.WithOptions.JsonGuildMfaLevel).ConfigureAwait(false)).Level;
    }

    public async Task<int> GetGuildPruneCountAsync(ulong guildId, int days, IEnumerable<ulong>? roles = null, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, roles == null ? $"/guilds/{guildId}/prune?days={days}" : $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildPruneCountResult.JsonGuildPruneCountResultSerializerContext.WithOptions.JsonGuildPruneCountResult).ConfigureAwait(false)).Pruned;

    public async Task<int?> GuildPruneAsync(ulong guildId, GuildPruneProperties pruneProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildPruneProperties>(pruneProperties, GuildPruneProperties.GuildPrunePropertiesSerializerContext.WithOptions.GuildPruneProperties))
            return (await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/prune", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildPruneResult.JsonGuildPruneResultSerializerContext.WithOptions.JsonGuildPruneResult).ConfigureAwait(false)).Pruned;
    }

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonVoiceRegion.JsonVoiceRegionArraySerializerContext.WithOptions.JsonVoiceRegionArray).ConfigureAwait(false)).Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", new RateLimits.Route(RateLimits.RouteParameter.GetGuildInvites), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonRestGuildInvite.JsonRestGuildInviteArraySerializerContext.WithOptions.JsonRestGuildInviteArray).ConfigureAwait(false)).Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<ulong, Integration>> GetGuildIntegrationsAsync(ulong guildId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonIntegration.JsonIntegrationArraySerializerContext.WithOptions.JsonIntegrationArray).ConfigureAwait(false)).ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(ulong guildId, ulong integrationId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", properties);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildWidgetSettings.JsonGuildWidgetSettingsSerializerContext.WithOptions.JsonGuildWidgetSettings).ConfigureAwait(false));

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(ulong guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? properties = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        using (HttpContent content = new JsonContent<GuildWidgetSettingsOptions>(guildWidgetSettingsOptions, GuildWidgetSettingsOptions.GuildWidgetSettingsOptionsSerializerContext.WithOptions.GuildWidgetSettingsOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/widget", new(RateLimits.RouteParameter.ModifyGuildWidgetSettings), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildWidgetSettings.JsonGuildWidgetSettingsSerializerContext.WithOptions.JsonGuildWidgetSettings).ConfigureAwait(false));
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildWidget.JsonGuildWidgetSerializerContext.WithOptions.JsonGuildWidget).ConfigureAwait(false), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildVanityInvite.JsonGuildVanityInviteSerializerContext.WithOptions.JsonGuildVanityInvite).ConfigureAwait(false));

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonWelcomeScreen.JsonWelcomeScreenSerializerContext.WithOptions.JsonWelcomeScreen).ConfigureAwait(false));

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(ulong guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? properties = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        using (HttpContent content = new JsonContent<GuildWelcomeScreenOptions>(guildWelcomeScreenOptions, GuildWelcomeScreenOptions.GuildWelcomeScreenOptionsSerializerContext.WithOptions.GuildWelcomeScreenOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/welcome-screen", new(RateLimits.RouteParameter.ModifyGuildWelcomeScreen), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonWelcomeScreen.JsonWelcomeScreenSerializerContext.WithOptions.JsonWelcomeScreen).ConfigureAwait(false));
    }

    public async Task<GuildOnboarding> GetGuildOnboardingAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/onboarding", new RateLimits.Route(RateLimits.RouteParameter.GetGuildOnboarding), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildOnboarding.JsonGuildOnboardingSerializerContext.WithOptions.JsonGuildOnboarding).ConfigureAwait(false), this);

    public async Task ModifyCurrentGuildUserVoiceStateAsync(ulong guildId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? properties = null)
    {
        CurrentUserVoiceStateOptions obj = new();
        action(obj);
        using (HttpContent content = new JsonContent<CurrentUserVoiceStateOptions>(obj, CurrentUserVoiceStateOptions.CurrentUserVoiceStateOptionsSerializerContext.WithOptions.CurrentUserVoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/voice-states/@me", new(RateLimits.RouteParameter.ModifyGuildUserVoiceState), content, properties).ConfigureAwait(false);
    }

    public async Task ModifyGuildUserVoiceStateAsync(ulong guildId, ulong channelId, ulong userId, Action<VoiceStateOptions> action, RequestProperties? properties = null)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        using (HttpContent content = new JsonContent<VoiceStateOptions>(obj, VoiceStateOptions.VoiceStateOptionsSerializerContext.WithOptions.VoiceStateOptions))
            await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/voice-states/{userId}", new(RateLimits.RouteParameter.ModifyGuildUserVoiceState), content, properties).ConfigureAwait(false);
    }
}
