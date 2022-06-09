using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildProperties), "/guilds", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> GetGuildAsync(Snowflake guildId, bool withCounts = false, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> ModifyGuildAsync(Snowflake guildId, Action<GuildOptions> action, RequestProperties? options = null)
    {
        GuildOptions guildProperties = new();
        action(guildProperties);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildProperties), $"/guilds/{guildId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);
    }

    public Task DeleteGuildAsync(Snowflake guildId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", options);

    public async IAsyncEnumerable<GuildBan> GetGuildBansAsync(Snowflake guildId, RequestProperties? options = null)
    {
        int count = 0;
        GuildBan? last = null;

        foreach (var ban in await GetMaxGuildBansAsyncTask(guildId, options).ConfigureAwait(false))
        {
            yield return last = ban;
            count++;
        }
        if (count == 1000)
        {
            await foreach (var ban in GetGuildBansAfterAsync(guildId, last!.User.Id, options))
                yield return ban;
        }
    }

    public async IAsyncEnumerable<GuildBan> GetGuildBansBeforeAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
    {
        int count;
        do
        {
            count = 0;
            foreach (var ban in await GetMaxGuildBansBeforeAsyncTask(guildId, userId, options).ConfigureAwait(false))
            {
                yield return ban;
                userId = ban.User.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    public async IAsyncEnumerable<GuildBan> GetGuildBansAfterAsync(Snowflake channelId, Snowflake userId, RequestProperties? options = null)
    {
        int count;
        do
        {
            count = 0;
            foreach (var ban in await GetMaxGuildBansAfterAsyncTask(channelId, userId, options).ConfigureAwait(false))
            {
                yield return ban;
                userId = ban.User.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAsyncTask(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().Select(b => new GuildBan(b, this));

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansBeforeAsyncTask(Snowflake guildId, Snowflake before, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?before={before}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().Select(b => new GuildBan(b, this)).Reverse();

    private async Task<IEnumerable<GuildBan>> GetMaxGuildBansAfterAsyncTask(Snowflake guildId, Snowflake after, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans?after={after}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().Select(b => new GuildBan(b, this));

    public async Task<GuildBan> GetGuildBanAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, GuildRole>> GetGuildRolesAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> CreateGuildRoleAsync(Snowflake guildId, GuildRoleProperties guildRoleProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildRoleProperties), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, GuildRole>> ModifyGuildRolePositionsAsync(Snowflake guildId, GuildRolePosition[] positions, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> ModifyGuildRoleAsync(Snowflake guildId, Snowflake roleId, Action<GuildRoleOptions> action, RequestProperties? options = null)
    {
        GuildRoleOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/roles/{roleId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);
    }

    public Task DeleteGuildRoleAsync(Snowflake guildId, Snowflake roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", options);

    public async Task<int> GetGuildPruneCountAsync(Snowflake guildId, int days, Snowflake[]? roles = null, RequestProperties? options = null)
    {
        if (roles == null)
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
        else
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
    }

    public async Task<int?> GuildPruneAsync(Snowflake guildId, GuildPruneProperties pruneProperties, RequestProperties? options = null)
        => JsonDocument.Parse(await SendRequestAsync(HttpMethod.Post, new JsonContent(pruneProperties), $"/guilds/{guildId}/prune", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").ToObject<int?>();

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonVoiceRegion>>().Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonRestGuildInvite>>().Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<Snowflake, Integration>> GetGuildIntegrationsAsync(Snowflake guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", options).ConfigureAwait(false))!.ToObject<IEnumerable<JsonModels.JsonIntegration>>().ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(Snowflake guildId, Snowflake integrationId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", options);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(Snowflake guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? options = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWidgetSettingsOptions), $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidget>(), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildVanityInvite>());

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(Snowflake guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? options = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWelcomeScreenOptions), $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());
    }

    public async Task<IReadOnlyDictionary<Snowflake, IGuildChannel>> GetGuildChannelsAsync(Snowflake guildId, RequestProperties? options = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel[]>().ToDictionary(c => c.Id, c => (IGuildChannel)NetCord.Channel.CreateFromJson(c, this));

    public async Task<IGuildChannel> CreateGuildChannelAsync(Snowflake guildId, GuildChannelProperties channelBuilder, RequestProperties? options = null)
        => (IGuildChannel)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, new JsonContent(channelBuilder), $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public async Task<Channel> ModifyGuildChannelAsync(Snowflake channelId, Action<GuildChannelOptions> action, RequestProperties? options = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);
    }

    public async Task<Channel> ModifyGuildThreadAsync(Snowflake channelId, Action<ThreadOptions> action, RequestProperties? options = null)
    {
        ThreadOptions threadOptions = new();
        action(threadOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(threadOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);
    }

    public Task ModifyGuildChannelPositionsAsync(Snowflake guildId, ChannelPosition[] positions, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/channels", options);

    public async Task<IReadOnlyDictionary<Snowflake, GuildThread>> GetActiveGuildThreadsAsync(Snowflake guildId, RequestProperties? options = null)
        => GuildThreadGenerator.CreateThreads((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonRestGuildThreadResult>(), this);

    public Task ModifyGuildChannelPermissionsAsync(Snowflake channelId, ChannelPermissionOverwrite permissionOverwrite, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent(permissionOverwrite), $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", options);

    public async Task<IEnumerable<RestGuildInvite>> GetGuildChannelInvitesAsync(Snowflake channelId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonRestGuildInvite>>().Select(r => new RestGuildInvite(r, this));

    public async Task<RestGuildInvite> CreateGuildChannelInviteAsync(Snowflake channelId, GuildInviteProperties? guildInviteProperties = null, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildInviteProperties), $"/channels/{channelId}/invites", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonRestGuildInvite>(), this);

    public Task DeleteGuildChannelPermissionAsync(Snowflake channelId, Snowflake overwriteId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", options);

    public async Task<FollowedChannel> FollowNewsGuildChannelAsync(Snowflake channelId, Snowflake targetChannelId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(@$"{{""webhook_channel_id"":{targetChannelId}}}"), $"/channels/{channelId}/followers", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonFollowedChannel>());

    public async Task<GuildUser> GetGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
    => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser>(), guildId, this);

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAsync(Snowflake guildId, RequestProperties? options = null)
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
            await foreach (var user in GetGuildUsersAfterAsync(guildId, lastUser!.Id, options))
                yield return user;
        }
    }

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAfterAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
    {
        short count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildUsersAfterAsyncTask(guildId, userId, options).ConfigureAwait(false))
            {
                yield return user;
                userId = user.Id;
                count++;
            }
        }
        while (count == 1000);
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAsyncTask(Snowflake guildId, RequestProperties? options = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonGuildUser>>().Select(u => new GuildUser(u, guildId, this));
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAfterAsyncTask(Snowflake guildId, Snowflake after, RequestProperties? options = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000&after={after}", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonGuildUser>>().Select(u => new GuildUser(u, guildId, this));
    }

    public async Task<IReadOnlyDictionary<Snowflake, GuildUser>> FindGuildUserAsync(Snowflake guildId, string name, int limit, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search?query={Uri.EscapeDataString(name)}&limit={limit}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser[]>().ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, this));

    public async Task<GuildUser?> AddGuildUserAsync(Snowflake guildId, Snowflake userId, UserProperties userProperties, RequestProperties? options = null)
    {
        var v = await SendRequestAsync(HttpMethod.Put, new JsonContent(userProperties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false);
        if (v == null)
            return null;
        else
            return new(v.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public async Task<GuildUser> ModifyGuildUserAsync(Snowflake guildId, Snowflake userId, Action<GuildUserProperties> action, RequestProperties? options = null)
    {
        GuildUserProperties properties = new();
        action(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public async Task<GuildUser> ModifyCurrentGuildUserAsync(Snowflake guildId, Action<CurrentGuildUserProperties> action, RequestProperties? options = null)
    {
        CurrentGuildUserProperties properties = new();
        action(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/@me", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public Task AddGuildUserRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

    public Task RemoveGuildUserRoleAsync(Snowflake guildId, Snowflake userId, Snowflake roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

    public Task KickGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", options);

    public Task BanGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", options);

    public Task BanGuildUserAsync(Snowflake guildId, Snowflake userId, int deleteMessageDays, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent($"{{\"delete_message_days\":{deleteMessageDays}}}"), $"/guilds/{guildId}/bans/{userId}", options);

    public Task UnbanGuildUserAsync(Snowflake guildId, Snowflake userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", options);

    public Task ModifyCurrentGuildUserVoiceStateAsync(Snowflake guildId, Snowflake channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? options = null)
    {
        CurrentUserVoiceStateOptions obj = new(channelId);
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/@me", options);
    }

    public Task ModifyGuildUserVoiceStateAsync(Snowflake guildId, Snowflake channelId, Snowflake userId, Action<VoiceStateOptions> action, RequestProperties? options = null)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/{userId}", options);
    }
}