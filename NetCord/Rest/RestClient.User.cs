namespace NetCord;

public partial class RestClient
{
    public async Task<SelfUser> GetCurrentUserAsync(RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, "/users/@me", options).ConfigureAwait(false)).ToObject<JsonModels.JsonUser>(), this);

    public async Task<User> GetUserAsync(Snowflake userId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonUser>(), this);

    public async Task<SelfUser> ModifyCurrentUserAsync(Action<SelfUserProperties> action, RequestProperties? options = null)
    {
        SelfUserProperties properties = new();
        action(properties);
        var result = (await SendRequestAsync(HttpMethod.Patch, new JsonContent(properties), $"/users/@me", options).ConfigureAwait(false))!;
        return new(result.ToObject<JsonModels.JsonUser>(), this);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(RequestProperties? options = null)
    {
        byte count = 0;
        RestGuild? last = null;
        foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds", options).ConfigureAwait(false))!
            .ToObject<IEnumerable<JsonModels.JsonGuild>>()
            .Select(g => new RestGuild(g, this)))
        {
            yield return last = guild;
            count++;
        }
        if (count == 200)
        {
            await foreach (var guild in GetCurrentUserGuildsAfterAsync(last!.Id, options))
                yield return guild;
        }
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAfterAsync(Snowflake guildId, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?after={after}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuild>>()
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                after = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async IAsyncEnumerable<RestGuild> GetCurrentUserGuildsBeforeAsync(Snowflake guildId, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var guild in (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds?before={before}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuild>>()
                .Select(g => new RestGuild(g, this)))
            {
                yield return guild;
                before = guild.Id;
                count++;
            }
        }
        while (count == 200);
    }

    public async Task<GuildUser> GetCurrentUserGuildUserAsync(Snowflake guildId, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", options).ConfigureAwait(false)).ToObject<JsonModels.JsonGuildUser>(), guildId, this);

    public Task LeaveGuildAsync(Snowflake guildId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", options);

    public async Task<DMChannel> GetDMChannelAsync(Snowflake userId, RequestProperties? options = null)
        => new DMChannel((await SendRequestAsync(HttpMethod.Post, new JsonContent($"{{\"recipient_id\":\"{userId}\"}}"), "/users/@me/channels", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonChannel>(), this);

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties properties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), "/users/@me/channels", options).ConfigureAwait(false)).ToObject<JsonModels.JsonChannel>(), this);

    public async Task<IReadOnlyDictionary<Snowflake, Connection>> GetUserConnectionsAsync(RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, "/users/@me/connections", options).ConfigureAwait(false)).ToObject<IEnumerable<JsonModels.JsonConnection>>().ToDictionary(c => c.Id, c => new Connection(c, this));
}