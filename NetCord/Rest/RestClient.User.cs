namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<SelfUser> GetCurrentUserAsync(RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/users/@me", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser), this);

    public async Task<User> GetUserAsync(ulong userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", new RateLimits.Route(RateLimits.RouteParameter.GetUser), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser), this);

    public async Task<SelfUser> ModifyCurrentUserAsync(Action<SelfUserOptions> action, RequestProperties? properties = null)
    {
        SelfUserOptions selfUserOptions = new();
        action(selfUserOptions);
        var result = (await SendRequestAsync(HttpMethod.Patch, $"/users/@me", new(RateLimits.RouteParameter.ModifyCurrentUser), new JsonContent<SelfUserOptions>(selfUserOptions, SelfUserOptions.SelfUserOptionsSerializerContext.WithOptions.SelfUserOptions), properties).ConfigureAwait(false))!;
        return new(result.ToObject(JsonModels.JsonUser.JsonUserSerializerContext.WithOptions.JsonUser), this);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(RequestProperties? properties = null)
    {
        byte count = 0;
        RestGuild? last = null;
        foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
            .ToObject(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray)
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
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?after={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
                .ToObject(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray)
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
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?before={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
                .ToObject(JsonModels.JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray)
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
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser), guildId, this);

    public Task LeaveGuildAsync(ulong guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", properties);

    public async Task<DMChannel> GetDMChannelAsync(ulong userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", new JsonContent<DMChannelProperties>(new(userId), DMChannelProperties.DMChannelPropertiesSerializerContext.WithOptions.DMChannelProperties), properties).ConfigureAwait(false))!.ToObject(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel), this);

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties groupDMChannelProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", new JsonContent<GroupDMChannelProperties>(groupDMChannelProperties, GroupDMChannelProperties.GroupDMChannelPropertiesSerializerContext.WithOptions.GroupDMChannelProperties), properties).ConfigureAwait(false)).ToObject(JsonModels.JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel), this);

    public async Task<IReadOnlyDictionary<ulong, Connection>> GetCurrentUserConnectionsAsync(RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, "/users/@me/connections", properties).ConfigureAwait(false)).ToObject(JsonModels.JsonConnection.JsonConnectionArraySerializerContext.WithOptions.JsonConnectionArray).ToDictionary(c => c.Id, c => new Connection(c, this));
}
