using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
    public async Task<RestGuild> CreateGuildAsync(GuildProperties guildProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildProperties), "/guilds", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> GetGuildAsync(DiscordId guildId, bool withCounts = false, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}?with_counts={withCounts}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<GuildPreview> GetGuildPreviewAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/preview", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);

    public async Task<RestGuild> ModifyGuildAsync(DiscordId guildId, Action<GuildOptions> action, RequestProperties? options = null)
    {
        GuildOptions guildProperties = new();
        action(guildProperties);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildProperties), $"/guilds/{guildId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuild>(), this);
    }

    public Task DeleteGuildAsync(DiscordId guildId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}", options);

    public async Task<IReadOnlyDictionary<DiscordId, GuildBan>> GetGuildBansAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan[]>().ToDictionary(b => b.User.Id, b => new GuildBan(b, this));

    public async Task<GuildBan> GetGuildBanAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/bans/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildBan>(), this);

    public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> GetGuildRolesAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> CreateGuildRoleAsync(DiscordId guildId, GuildRoleProperties guildRoleProperties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildRoleProperties), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);

    public async Task<IReadOnlyDictionary<DiscordId, GuildRole>> ModifyGuildRolePositionsAsync(DiscordId guildId, GuildRolePosition[] positions, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/roles", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole[]>().ToDictionary(r => r.Id, r => new GuildRole(r, this));

    public async Task<GuildRole> ModifyGuildRoleAsync(DiscordId guildId, DiscordId roleId, Action<GuildRoleOptions> action, RequestProperties? options = null)
    {
        GuildRoleOptions obj = new();
        action(obj);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/roles/{roleId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildRole>(), this);
    }

    public Task DeleteGuildRoleAsync(DiscordId guildId, DiscordId roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/roles/{roleId}", options);

    public async Task<int> GetGuildPruneCountAsync(DiscordId guildId, int days, DiscordId[]? roles = null, RequestProperties? options = null)
    {
        if (roles == null)
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
        else
            return JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/prune?days={days}&include_roles={string.Join(',', roles)}", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").GetInt32();
    }

    public async Task<int?> GuildPruneAsync(DiscordId guildId, GuildPruneProperties pruneProperties, RequestProperties? options = null)
        => JsonDocument.Parse(await SendRequestAsync(HttpMethod.Post, new JsonContent(pruneProperties), $"/guilds/{guildId}/prune", options).ConfigureAwait(false))!.RootElement.GetProperty("pruned").ToObject<int?>();

    public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegionsAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/regions", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonVoiceRegion>>().Select(r => new VoiceRegion(r));

    public async Task<IEnumerable<RestGuildInvite>> GetGuildInvitesAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/invites", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonRestGuildInvite>>().Select(i => new RestGuildInvite(i, this));

    public async Task<IReadOnlyDictionary<DiscordId, Integration>> GetGuildIntegrationsAsync(DiscordId guildId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/integrations", options).ConfigureAwait(false))!.ToObject<IEnumerable<JsonModels.JsonIntegration>>().ToDictionary(i => i.Id, i => new Integration(i, this));

    public Task DeleteGuildIntegrationAsync(DiscordId guildId, DiscordId integrationId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/integrations/{integrationId}", options);

    public async Task<GuildWidgetSettings> GetGuildWidgetSettingsAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());

    public async Task<GuildWidgetSettings> ModifyGuildWidgetSettingsAsync(DiscordId guildId, Action<GuildWidgetSettingsOptions> action, RequestProperties? options = null)
    {
        GuildWidgetSettingsOptions guildWidgetSettingsOptions = new();
        action(guildWidgetSettingsOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWidgetSettingsOptions), $"/guilds/{guildId}/widget", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidgetSettings>());
    }

    public async Task<GuildWidget> GetGuildWidgetAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/widget.json", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildWidget>(), this);

    public async Task<GuildVanityInvite> GetGuildVanityInviteAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/vanity-url", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildVanityInvite>());

    public async Task<GuildWelcomeScreen> GetGuildWelcomeScreenAsync(DiscordId guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());

    public async Task<GuildWelcomeScreen> ModifyGuildWelcomeScreenAsync(DiscordId guildId, Action<GuildWelcomeScreenOptions> action, RequestProperties? options = null)
    {
        GuildWelcomeScreenOptions guildWelcomeScreenOptions = new();
        action(guildWelcomeScreenOptions);
        return new((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildWelcomeScreenOptions), $"/guilds/{guildId}/welcome-screen", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonWelcomeScreen>());
    }

    public async Task<IReadOnlyDictionary<DiscordId, IGuildChannel>> GetGuildChannelsAsync(DiscordId guildId, RequestProperties? options = null)
    => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel[]>().ToDictionary(c => c.Id, c => (IGuildChannel)NetCord.Channel.CreateFromJson(c, this));

    public async Task<IGuildChannel> CreateGuildChannelAsync(DiscordId guildId, GuildChannelProperties channelBuilder, RequestProperties? options = null)
        => (IGuildChannel)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, new JsonContent(channelBuilder), $"/guilds/{guildId}/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public async Task<Channel> ModifyGuildChannelAsync(DiscordId channelId, Action<GuildChannelOptions> action, RequestProperties? options = null)
    {
        GuildChannelOptions guildChannelOptions = new();
        action(guildChannelOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(guildChannelOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);
    }

    public async Task<Channel> ModifyGuildThreadAsync(DiscordId channelId, Action<ThreadOptions> action, RequestProperties? options = null)
    {
        ThreadOptions threadOptions = new();
        action(threadOptions);
        return Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Patch, new JsonContent(threadOptions), $"/channels/{channelId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);
    }

    public Task ModifyGuildChannelPositionsAsync(DiscordId guildId, ChannelPosition[] positions, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Patch, new JsonContent(positions), $"/guilds/{guildId}/channels", options);

    public Task ModifyGuildChannelPermissionsAsync(DiscordId channelId, ChannelPermissionOverwrite permissionOverwrite, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent(permissionOverwrite), $"/channels/{channelId}/permissions/{permissionOverwrite.Id}", options);

    public async Task<IEnumerable<RestGuildInvite>> GetGuildChannelInvitesAsync(DiscordId channelId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/invites", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonRestGuildInvite>>().Select(r => new RestGuildInvite(r, this));

    public async Task<RestGuildInvite> CreateGuildChannelInviteAsync(DiscordId channelId, GuildInviteProperties? guildInviteProperties = null, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(guildInviteProperties), $"/channels/{channelId}/invites", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonRestGuildInvite>(), this);

    public Task DeleteGuildChannelPermissionAsync(DiscordId channelId, DiscordId overwriteId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{channelId}/permissions/{overwriteId}", options);

    public async Task<FollowedChannel> FollowNewsGuildChannelAsync(DiscordId channelId, DiscordId targetChannelId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(@$"{{""webhook_channel_id"":{targetChannelId}}}"), $"/channels/{channelId}/followers", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonFollowedChannel>());

    public async Task<GuildThread> CreateGuildThreadAsync(DiscordId channelId, DiscordId messageId, ThreadWithMessageProperties properties, RequestProperties? options = null)
        => (GuildThread)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/channels/{channelId}/messages/{messageId}/threads", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public async Task<GuildThread> CreateGuildThreadAsync(DiscordId channelId, ThreadProperties properties, RequestProperties? options = null)
        => (GuildThread)Channel.CreateFromJson((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/channels/{channelId}/threads", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public Task JoinGuildThreadAsync(DiscordId threadId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/@me", options);

    public Task AddGuildThreadUser(DiscordId threadId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/channels/{threadId}/thread-members/{userId}", options);

    public Task LeaveGuildThreadAsync(DiscordId threadId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/@me", options);

    public Task DeleteGuildThreadUser(DiscordId threadId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/channels/{threadId}/thread-members/{userId}", options);

    public async Task<ThreadUser> GetGuildThreadUserAsync(DiscordId threadId, DiscordId userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonThreadUser>(), this);

    public async Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetGuildThreadUsersAsync(DiscordId threadId, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/channels/{threadId}/thread-members", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonThreadUser[]>().ToDictionary(u => u.UserId, u => new ThreadUser(u, this));

    public async Task<(IReadOnlyDictionary<DiscordId, GuildThread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetActiveGuildThreadsAsync(DiscordId guildId, RequestProperties? options = null)
    {
        var json = JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/threads/active", options).ConfigureAwait(false))!.RootElement;
        var threads = json.GetProperty("threads").ToObject<JsonModels.JsonChannel[]>();
        var users = json.GetProperty("members").ToObject<JsonModels.JsonThreadUser[]>();
        return (threads.ToDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, this)), users.ToDictionary(u => u.ThreadId, u => new ThreadUser(u, this)));
    }

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(DiscordId channelId, RequestProperties? options = null)
    {
        string url = $"/channels/{channelId}/threads/archived/public?limit=100";
        var json = JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, url, options).ConfigureAwait(false))!.RootElement;
        var threads = json.GetProperty("threads");
        var users = json.GetProperty("members");
        GuildThread? last = null;
        foreach (var t in threads.EnumerateArray().Zip(users.EnumerateArray()))
            yield return last = (GuildThread)Channel.CreateFromJson(t.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = t.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, this);
        await foreach (var v in GetPublicArchivedGuildThreadsAsync(channelId, last!.Metadata.ArchiveTimestamp, options))
            yield return v;
    }

    public async IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(DiscordId channelId, DateTimeOffset before, RequestProperties? options = null)
    {
        string url = $"/channels/{channelId}/threads/archived/public?limit=100&before=";
        var json = JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"{url}{before:s}", options).ConfigureAwait(false))!.RootElement;
        var threads = json.GetProperty("threads");
        var users = json.GetProperty("members");
        GuildThread? last = null;
        foreach (var v in threads.EnumerateArray().Zip(users.EnumerateArray()))
            yield return last = (GuildThread)Channel.CreateFromJson(v.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = v.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, this);
        while (json.GetProperty("has_more").GetBoolean())
        {
            json = JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"{url}{last!.Metadata.ArchiveTimestamp:s}", options).ConfigureAwait(false))!.RootElement;
            threads = json.GetProperty("threads");
            users = json.GetProperty("members");
            foreach (var v in threads.EnumerateArray().Zip(users.EnumerateArray()))
                yield return last = (GuildThread)Channel.CreateFromJson(v.First.ToObject<JsonModels.JsonChannel>() with { CurrentUser = v.Second.ToObject<JsonModels.JsonThreadSelfUser>() }, this);
        }
    }

    public async Task<(IReadOnlyDictionary<DiscordId, GuildThread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetPrivateArchivedGuildThreadsAsync(DiscordId channelId, RequestProperties? options = null)
    {
        var json = JsonDocument.Parse(await SendRequestAsync(HttpMethod.Get, $"/channels/{channelId}/threads/archived/private", options).ConfigureAwait(false))!.RootElement;
        var threads = json.GetProperty("threads").ToObject<JsonModels.JsonChannel[]>();
        var users = json.GetProperty("members").ToObject<JsonModels.JsonThreadUser[]>();
        return (threads.ToDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, this)), users.ToDictionary(u => u.ThreadId, u => new ThreadUser(u, this)));
    }

    public async Task<GuildUser> GetGuildUserAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
    => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser>(), guildId, this);

    public async IAsyncEnumerable<GuildUser> GetGuildUsersAsync(DiscordId guildId, RequestProperties? options = null)
    {
        short count = 0;
        GuildUser? lastUser = null;

        foreach (var user in await GetMaxGuildUsersAsyncTask(guildId).ConfigureAwait(false))
        {
            yield return lastUser = user;
            count++;
        }
        while (count == 1000)
        {
            count = 0;
            foreach (var user in await GetMaxGuildUsersAfterAsyncTask(guildId, lastUser!, options).ConfigureAwait(false))
            {
                yield return lastUser = user;
                count++;
            }
        }
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAsyncTask(DiscordId guildId, RequestProperties? options = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonGuildUser>>().Select(u => new GuildUser(u, guildId, this));
    }

    private async Task<IEnumerable<GuildUser>> GetMaxGuildUsersAfterAsyncTask(DiscordId guildId, DiscordId after, RequestProperties? options = null)
    {
        return (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members?limit=1000&after={after}", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonGuildUser>>().Select(u => new GuildUser(u, guildId, this));
    }

    public async Task<IReadOnlyDictionary<DiscordId, GuildUser>> FindGuildUserAsync(DiscordId guildId, string name, int limit, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/members/search?query={Uri.EscapeDataString(name)}&limit={limit}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildUser[]>().ToDictionary(u => u.User.Id, u => new GuildUser(u, guildId, this));

    public async Task<GuildUser?> AddGuildUserAsync(DiscordId guildId, DiscordId userId, UserProperties userProperties, RequestProperties? options = null)
    {
        var v = await SendRequestAsync(HttpMethod.Put, new JsonContent(userProperties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false);
        if (v == null)
            return null;
        else
            return new(v.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public async Task<GuildUser> ModifyGuildUserAsync(DiscordId guildId, DiscordId userId, Action<GuildUserProperties> action, RequestProperties? options = null)
    {
        GuildUserProperties properties = new();
        action(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/{userId}", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public async Task<GuildUser> ModifyCurrentGuildUserAsync(DiscordId guildId, Action<CurrentGuildUserProperties> action, RequestProperties? options = null)
    {
        CurrentGuildUserProperties properties = new();
        action(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/guilds/{guildId}/members/@me", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonGuildUser>(), guildId, this);
    }

    public Task AddGuildUserRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

    public Task RemoveGuildUserRoleAsync(DiscordId guildId, DiscordId userId, DiscordId roleId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}/roles/{roleId}", options);

    public Task KickGuildUserAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/members/{userId}", options);

    public Task BanGuildUserAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, $"/guilds/{guildId}/bans/{userId}", options);

    public Task BanGuildUserAsync(DiscordId guildId, DiscordId userId, int deleteMessageDays, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Put, new JsonContent($"{{\"delete_message_days\":{deleteMessageDays}}}"), $"/guilds/{guildId}/bans/{userId}", options);

    public Task UnbanGuildUserAsync(DiscordId guildId, DiscordId userId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/bans/{userId}", options);

    public Task ModifyCurrentGuildUserVoiceStateAsync(DiscordId guildId, DiscordId channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? options = null)
    {
        CurrentUserVoiceStateOptions obj = new(channelId);
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/@me", options);
    }

    public Task ModifyGuildUserVoiceStateAsync(DiscordId guildId, DiscordId channelId, DiscordId userId, Action<VoiceStateOptions> action, RequestProperties? options = null)
    {
        VoiceStateOptions obj = new(channelId);
        action(obj);
        return SendRequestAsync(HttpMethod.Patch, new JsonContent(obj), $"/guilds/{guildId}/voice-states/{userId}", options);
    }
}