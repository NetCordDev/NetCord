namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<SelfUser> GetCurrentUserAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, "/users/@me", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);

    public async Task<User> GetUserAsync(ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", new RateLimits.Route(RateLimits.RouteParameter.GetUser), properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);

    public async Task<SelfUser> ModifyCurrentUserAsync(Action<SelfUserOptions> action, RequestProperties? properties = null)
    {
        SelfUserOptions selfUserOptions = new();
        action(selfUserOptions);
        using (HttpContent content = new JsonContent<SelfUserOptions>(selfUserOptions, SelfUserOptions.SelfUserOptionsSerializerContext.WithOptions.SelfUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/users/@me", new(RateLimits.RouteParameter.ModifyCurrentUser), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(RequestProperties? properties = null)
    {
        byte count = 0;
        RestGuild? last = null;
        foreach (var guild in (await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))
            .ToObjectAsync(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray).ConfigureAwait(false))
            .Select(g => new RestGuild(g, this)))
        {
            yield return last = guild;
            count++;
        }
        if (count == 200)
        {
            await foreach (var guild in GetCurrentUserGuildsAfterAsync(last!.Id, properties))
                yield return guild;
        }
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAfterAsync(ulong guildId, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?after={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))
                .ToObjectAsync(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray).ConfigureAwait(false))
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                guildId = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsBeforeAsync(ulong guildId, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?before={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))
                .ToObjectAsync(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray).ConfigureAwait(false))
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                guildId = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async Task<GuildUser> GetCurrentUserGuildUserAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);

    public Task LeaveGuildAsync(ulong guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", properties);

    public async Task<DMChannel> GetDMChannelAsync(ulong userId, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<DMChannelProperties>(new(userId), DMChannelProperties.DMChannelPropertiesSerializerContext.WithOptions.DMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties groupDMChannelProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GroupDMChannelProperties>(groupDMChannelProperties, GroupDMChannelProperties.GroupDMChannelPropertiesSerializerContext.WithOptions.GroupDMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<IReadOnlyDictionary<ulong, Connection>> GetCurrentUserConnectionsAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, "/users/@me/connections", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonConnection.JsonConnectionArraySerializerContext.WithOptions.JsonConnectionArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new Connection(c, this));

    public async Task<ApplicationRoleConnection> GetCurrentUserApplicationRoleConnectionAsync(ulong applicationId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/applications/{applicationId}/role-connection", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonApplicationRoleConnection.JsonApplicationRoleConnectionSerializerContext.WithOptions.JsonApplicationRoleConnection).ConfigureAwait(false));

    public async Task<ApplicationRoleConnection> UpdateCurrentUserApplicationRoleConnectionAsync(ulong applicationId, ApplicationRoleConnectionProperties applicationRoleConnectionProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<ApplicationRoleConnectionProperties>(applicationRoleConnectionProperties, ApplicationRoleConnectionProperties.ApplicationRoleConnectionPropertiesSerializerContext.WithOptions.ApplicationRoleConnectionProperties))
            return new(await (await SendRequestAsync(HttpMethod.Put, $"/users/@me/applications/{applicationId}/role-connection", content, properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonApplicationRoleConnection.JsonApplicationRoleConnectionSerializerContext.WithOptions.JsonApplicationRoleConnection).ConfigureAwait(false));
    }
}
