namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/guilds", new JsonContent<GuildProperties>(guildProperties, GuildProperties.GuildPropertiesSerializerContext.WithOptions.GuildProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), this);

    public async Task<RestGuild> GetGuildAsync(Snowflake guildId, bool withCounts = false, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), this);

    public async Task<RestGuild> ModifyGuildAsync(Snowflake guildId, Action<GuildOptions> action, RequestProperties? properties = null)
    {
        GuildOptions guildProperties = new();
        action(guildProperties);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}", new(RateLimits.RouteParameter.Guilds), new JsonContent<GuildOptions>(guildProperties, GuildOptions.GuildOptionsSerializerContext.WithOptions.GuildOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuild.JsonGuildSerializerContext.WithOptions.JsonGuild), this);
    }

    public Task DeleteGuildAsync(Snowflake guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", properties);

    public async IAsyncEnumerable<GuildBan> GetGuildBansAsync(Snowflake guildId, RequestProperties? properties = null)
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

    public async IAsyncEnumerable<GuildBan> GetGuildBansBeforeAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
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

    public async IAsyncEnumerable<GuildBan> GetGuildBansAfterAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
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

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAsyncTask(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).Select(b => new GuildBan(b, guildId, this));

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansBeforeAsyncTask(Snowflake guildId, Snowflake before, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?before={before}", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).Select(b => new GuildBan(b, guildId, this)).Reverse();

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAfterAsyncTask(Snowflake guildId, Snowflake after, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?after={after}", new RateLimits.Route(RateLimits.RouteParameter.GuildBans), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildBan.JsonGuildBanArraySerializerContext.WithOptions.JsonGuildBanArray).Select(b => new GuildBan(b, guildId, this));

    public async Task<GuildBan> GetGuildBanAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildBan.JsonGuildBanSerializerContext.WithOptions.JsonGuildBan), guildId, this);

    public async Task<IReadOnlyDictionary<Snowflake, GuildRole>> GetGuildRolesAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildRole.JsonGuildRoleArraySerializerContext.WithOptions.JsonGuildRoleArray).ToDictionary(r => r.Id, r => new GuildRole(r, guildId, this));

    public async Task<GuildRole> CreateGuildRoleAsync(Snowflake guildId, GuildRoleProperties guildRoleProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/roles", new(RateLimits.RouteParameter.CreateGuildRole), new JsonContent<GuildRoleProperties>(guildRoleProperties, GuildRoleProperties.GuildRolePropertiesSerializerContext.WithOptions.GuildRoleProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildRole.JsonGuildRoleSerializerContext.WithOptions.JsonGuildRole), guildId, this);

    public async Task<IReadOnlyDictionary<Snowflake, GuildRole>> ModifyGuildRolePositionsAsync(Snowflake guildId, IEnumerable<GuildRolePositionProperties> positions, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles", new(RateLimits.RouteParameter.ModifyGuildRolePositions), new JsonContent<IEnumerable<GuildRolePositionProperties>>(positions, GuildRolePositionProperties.IEnumerableOfGuildRolePositionPropertiesSerializerContext.WithOptions.IEnumerableGuildRolePositionProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildRole.JsonGuildRoleArraySerializerContext.WithOptions.JsonGuildRoleArray).ToDictionary(r => r.Id, r => new GuildRole(r, guildId, this));

    public async Task<GuildRole> ModifyGuildRoleAsync(Snowflake guildId, Snowflake roleId, Action<GuildRoleOptions> action, RequestProperties? properties = null)
    {
        GuildRoleOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/roles/{roleId}", new(RateLimits.RouteParameter.ModifyGuildRole), new JsonContent<GuildRoleOptions>(obj, GuildRoleOptions.GuildRoleOptionsSerializerContext.WithOptions.GuildRoleOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildRole.JsonGuildRoleSerializerContext.WithOptions.JsonGuildRole), guildId, this);
    }

    public Task DeleteGuildRoleAsync(Snowflake guildId, Snowflake roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", properties);

    public async Task<MfaLevel> ModifyGuildMfaLevelAsync(Snowflake guildId, MfaLevel mfaLevel, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/mfa", new JsonContent<GuildMfaLevelProperties>(new GuildMfaLevelProperties(mfaLevel), GuildMfaLevelProperties.GuildMfaLevelPropertiesSerializerContext.WithOptions.GuildMfaLevelProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildMfaLevel.JsonGuildMfaLevelSerializerContext.WithOptions.JsonGuildMfaLevel).Level;

    public async Task<int> GetGuildPruneCountAsync(Snowflake guildId, int days, IEnumerable<Snowflake>? roles = null, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, roles == null ? $"/guilds/{guildId}/prune?days={days}" : $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildPruneCountResult.JsonGuildPruneCountResultSerializerContext.WithOptions.JsonGuildPruneCountResult).Pruned;

    public async Task<int?> GuildPruneAsync(Snowflake guildId, GuildPruneProperties pruneProperties, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/prune", new JsonContent<GuildPruneProperties>(pruneProperties, GuildPruneProperties.GuildPrunePropertiesSerializerContext.WithOptions.GuildPruneProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildPruneResult.JsonGuildPruneResultSerializerContext.WithOptions.JsonGuildPruneResult).Pruned;

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonVoiceRegion.JsonVoiceRegionArraySerializerContext.WithOptions.JsonVoiceRegionArray).Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", new RateLimits.Route(RateLimits.RouteParameter.GetGuildInvites), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonRestGuildInvite.JsonRestGuildInviteArraySerializerContext.WithOptions.JsonRestGuildInviteArray).Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<Snowflake, Integration>> GetGuildIntegrationsAsync(Snowflake guildId, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonIntegration.JsonIntegrationArraySerializerContext.WithOptions.JsonIntegrationArray).ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(Snowflake guildId, Snowflake integrationId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", properties);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildWidgetSettings.JsonGuildWidgetSettingsSerializerContext.WithOptions.JsonGuildWidgetSettings));

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(Snowflake guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? properties = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/widget", new(RateLimits.RouteParameter.ModifyGuildWidgetSettings), new JsonContent<GuildWidgetSettingsOptions>(guildWidgetSettingsOptions, GuildWidgetSettingsOptions.GuildWidgetSettingsOptionsSerializerContext.WithOptions.GuildWidgetSettingsOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildWidgetSettings.JsonGuildWidgetSettingsSerializerContext.WithOptions.JsonGuildWidgetSettings));
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildWidget.JsonGuildWidgetSerializerContext.WithOptions.JsonGuildWidget), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildVanityInvite.JsonGuildVanityInviteSerializerContext.WithOptions.JsonGuildVanityInvite));

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonWelcomeScreen.JsonWelcomeScreenSerializerContext.WithOptions.JsonWelcomeScreen));

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(Snowflake guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? properties = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/welcome-screen", new(RateLimits.RouteParameter.ModifyGuildWelcomeScreen), new JsonContent<GuildWelcomeScreenOptions>(guildWelcomeScreenOptions, GuildWelcomeScreenOptions.GuildWelcomeScreenOptionsSerializerContext.WithOptions.GuildWelcomeScreenOptions), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonWelcomeScreen.JsonWelcomeScreenSerializerContext.WithOptions.JsonWelcomeScreen));
    }

    public async Task<IReadOnlyDictionary<Snowflake, IGuildChannel>> GetGuildChannelsAsync(Snowflake guildId, RequestProperties? properties = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", new RateLimits.Route(RateLimits.RouteParameter.GetGuildChannels), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonChannel.JsonChannelArraySerializerContext.WithOptions.JsonChannelArray).ToDictionary(c => c.Id, c => (IGuildChannel)NetCord.Channel.CreateFromJson(c, this));

    public async Task<IGuildChannel> CreateGuildChannelAsync(Snowflake guildId, GuildChannelProperties channelProperties, RequestProperties? properties = null)
        => (IGuildChannel)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/channels", new(RateLimits.RouteParameter.GuildChannels), new JsonContent<GuildChannelProperties>(channelProperties, GuildChannelProperties.GuildChannelPropertiesSerializerContext.WithOptions.GuildChannelProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel), this);

    public Task ModifyGuildChannelPositionsAsync(Snowflake guildId, IEnumerable<ChannelPositionProperties> positions, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/channels", new(RateLimits.RouteParameter.GuildChannelPositions), new JsonContent<IEnumerable<ChannelPositionProperties>>(positions, ChannelPositionProperties.IEnumerableOfChannelPositionPropertiesSerializerContext.WithOptions.IEnumerableChannelPositionProperties), properties);

    public async Task<IReadOnlyDictionary<Snowflake, GuildThread>> GetActiveGuildThreadsAsync(Snowflake guildId, RequestProperties? properties = null)
        => GuildThreadGenerator.CreateThreads((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonRestGuildThreadResult.JsonRestGuildThreadResultSerializerContext.WithOptions.JsonRestGuildThreadResult), this);

    public async Task<GuildUser> GetGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUser), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser), guildId, this);

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAsync(Snowflake guildId, RequestProperties? properties = null)
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

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAfterAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
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

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAsyncTask(Snowflake guildId, RequestProperties? properties = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUsers), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).Select(u => new GuildUser(u, guildId, this));
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAfterAsyncTask(Snowflake guildId, Snowflake after, RequestProperties? properties = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000&after={after}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildUsers), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).Select(u => new GuildUser(u, guildId, this));
    }

    public async Task<IReadOnlyDictionary<Snowflake, GuildUser>> FindGuildUserAsync(Snowflake guildId, string name, int limit, RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search?query={Uri.EscapeDataString(name)}&limit={limit}", new RateLimits.Route(RateLimits.RouteParameter.FindGuildUser), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonGuildUser.JsonGuildUserArraySerializerContext.WithOptions.JsonGuildUserArray).ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, this));

    public async Task<GuildUser?> AddGuildUserAsync(Snowflake guildId, Snowflake userId, GuildUserProperties userProperties, RequestProperties? properties = null)
    {
        var v = await SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}", new(RateLimits.RouteParameter.AddGuildUser), new JsonContent<GuildUserProperties>(userProperties, GuildUserProperties.GuildUserPropertiesSerializerContext.WithOptions.GuildUserProperties), properties).ConfigureAwait(false);
        if (v == null)
            return null;
        else
            return new(v.ToObject(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser), guildId, this);
    }

    public async Task<GuildUser> ModifyGuildUserAsync(Snowflake guildId, Snowflake userId, Action<GuildUserOptions> action, RequestProperties? properties = null)
    {
        GuildUserOptions guildUserOptions = new();
        action(guildUserOptions);
        var result = (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/members/{userId}", new(RateLimits.RouteParameter.ModifyGuildUser), new JsonContent<GuildUserOptions>(guildUserOptions, GuildUserOptions.GuildUserOptionsSerializerContext.WithOptions.GuildUserOptions), properties).ConfigureAwait(false))!;
        return new(result.ToObject(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser), guildId, this);
    }

    public async Task<GuildUser> ModifyCurrentGuildUserAsync(Snowflake guildId, Action<CurrentGuildUserOptions> action, RequestProperties? properties = null)
    {
        CurrentGuildUserOptions currentGuildUserOptions = new();
        action(currentGuildUserOptions);
        var result = (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/members/@me", new(RateLimits.RouteParameter.ModifyCurrentGuildUser), new JsonContent<CurrentGuildUserOptions>(currentGuildUserOptions, CurrentGuildUserOptions.CurrentGuildUserOptionsSerializerContext.WithOptions.CurrentGuildUserOptions), properties).ConfigureAwait(false))!;
        return new(result.ToObject(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser), guildId, this);
    }

    public Task AddGuildUserRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", new RateLimits.Route(RateLimits.RouteParameter.AddRemoveGuildUserRole), properties);

    public Task RemoveGuildUserRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", new RateLimits.Route(RateLimits.RouteParameter.AddRemoveGuildUserRole), properties);

    public Task KickGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", new RateLimits.Route(RateLimits.RouteParameter.KickGuildUser), properties);

    public Task BanGuildUserAsync(Snowflake guildId, Snowflake userId, int deleteMessageSeconds = 0, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", new JsonContent<GuildBanProperties>(new(deleteMessageSeconds), GuildBanProperties.GuildBanPropertiesSerializerContext.WithOptions.GuildBanProperties), properties);

    public Task UnbanGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", properties);

    public Task ModifyCurrentGuildUserVoiceStateAsync(Snowflake guildId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? properties = null)
    {
        CurrentUserVoiceStateOptions obj = new();
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/voice-states/@me", new(RateLimits.RouteParameter.ModifyGuildUserVoiceState), new JsonContent<CurrentUserVoiceStateOptions>(obj, CurrentUserVoiceStateOptions.CurrentUserVoiceStateOptionsSerializerContext.WithOptions.CurrentUserVoiceStateOptions), properties);
    }

    public Task ModifyGuildUserVoiceStateAsync(Snowflake guildId, Snowflake channelId, Snowflake userId, Action<VoiceStateOptions> action, RequestProperties? properties = null)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/voice-states/{userId}", new(RateLimits.RouteParameter.ModifyGuildUserVoiceState), new JsonContent<VoiceStateOptions>(obj, VoiceStateOptions.VoiceStateOptionsSerializerContext.WithOptions.VoiceStateOptions), properties);
    }
}
