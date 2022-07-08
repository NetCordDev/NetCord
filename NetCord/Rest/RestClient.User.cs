namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<SelfUser> GetCurrentUserAsync(RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/users/@me", properties).ConfigureAwait(false)).ToObject<JsonModels.JsonUser>(), this);

    public async Task<User> GetUserAsync(Snowflake userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", new RateLimits.Route(RateLimits.RouteParameter.GetUser), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonUser>(), this);

    public async Task<SelfUser> ModifyCurrentUserAsync(Action<SelfUserProperties> action, RequestProperties? properties = null)
    {
        SelfUserProperties selfUserProperties = new();
        action(selfUserProperties);
        var result = (await SendRequestAsync(HttpMethod.Patch, $"/users/@me", new(RateLimits.RouteParameter.ModifyCurrentUser), new JsonContent(selfUserProperties), properties).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonUser>(), this);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(RequestProperties? properties = null)
    {
        byte count = 0;
        RestGuild? last = null;
        foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
            .ToObject<JsonModels.JsonGuild[]>()
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

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAfterAsync(Snowflake guildId, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?after={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
                .ToObject<JsonModels.JsonGuild[]>()
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                guildId = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsBeforeAsync(Snowflake guildId, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?before={guildId}", new RateLimits.Route(RateLimits.RouteParameter.GetCurrentUserGuilds), properties).ConfigureAwait(false))!
                .ToObject<JsonModels.JsonGuild[]>()
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                guildId = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async Task<GuildUser> GetCurrentUserGuildUserAsync(Snowflake guildId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", properties).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildUser>(), guildId, this);

    public Task LeaveGuildAsync(Snowflake guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", properties);

    public async Task<DMChannel> GetDMChannelAsync(Snowflake userId, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", new JsonContent($"{{\"recipient_id\":\"{userId}\"}}"), properties).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties groupDMChannelProperties, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Post, "/users/@me/channels", new JsonContent(groupDMChannelProperties), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonChannel>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, Connection>> GetUserConnectionsAsync(RequestProperties? properties = null)
        => (await SendRequestAsync(HttpMethod.Get, "/users/@me/connections", properties).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonConnection>>().ToDictionary(c => c.Id, c => new Connection(c, this));
}