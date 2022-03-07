using System.Text.Json;

namespace NetCord;

public partial class RestClient
{
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
}