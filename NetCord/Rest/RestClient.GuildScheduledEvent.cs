namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<Snowflake, GuildScheduledEvent>> GetGuildScheduledEventsAsync(Snowflake guildId, bool withUserCount = false, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events?with_user_count={withUserCount}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent[]>().ToDictionary(e => e.Id, e => new GuildScheduledEvent(e, this));

    public async Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(Snowflake guildId, GuildScheduledEventProperties properties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/guilds/{guildId}/scheduled-events", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);

    public async Task<GuildScheduledEvent> GetGuildScheduledEventAsync(Snowflake guildId, Snowflake scheduledEventId, bool withUserCount = false, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}?with_user_count={withUserCount}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);

    public async Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(Snowflake guildId, Snowflake scheduledEventId, Action<GuildScheduledEventOptions> action, RequestProperties? options = null)
    {
        GuildScheduledEventOptions o = new();
        action(o);
        return new((await SendRequestAsync(HttpMethod.Get, new JsonContent(o), $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);
    }

    public Task DeleteGuildScheduledEventAsync(Snowflake guildId, Snowflake scheduledEventId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", options);

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAsync(Snowflake guildId, Snowflake scheduledEventId, bool guildUsers = false, RequestProperties? options = null)
    {
        byte count = 0;
        GuildScheduledEventUser? last = null;
        foreach (var user in await GetMaxGuildScheduledEventUsersAsyncTask(guildId, scheduledEventId, guildUsers, options).ConfigureAwait(false))
        {
            yield return last = user;
            count++;
        }
        if (count == 100)
        {
            await foreach (var user in GetGuildScheduledEventUsersAfterAsync(guildId, scheduledEventId, last!.User.Id, guildUsers, options))
                yield return user;
        }
    }

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAfterAsync(Snowflake guildId, Snowflake scheduledEventId, Snowflake userId, bool guildUsers = false, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildScheduledEventUsersAfterAsyncTask(guildId, scheduledEventId, userId, guildUsers, options).ConfigureAwait(false))
            {
                yield return user;
                userId = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersBeforeAsync(Snowflake guildId, Snowflake scheduledEventId, Snowflake userId, bool guildUsers = false, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildScheduledEventUsersBeforeAsyncTask(guildId, scheduledEventId, userId, guildUsers, options).ConfigureAwait(false))
            {
                yield return user;
                userId = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersAsyncTask(Snowflake guildId, Snowflake scheduledEventId, bool guildUsers, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?with_member={guildUsers}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuildScheduledEventUser>>()
                .Select(u => new GuildScheduledEventUser(u, guildId, this));

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersAfterAsyncTask(Snowflake guildId, Snowflake scheduledEventId, Snowflake after, bool guildUsers, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?after={after}&with_member={guildUsers}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuildScheduledEventUser>>()
                .Select(u => new GuildScheduledEventUser(u, guildId, this));

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersBeforeAsyncTask(Snowflake guildId, Snowflake scheduledEventId, Snowflake before, bool guildUsers, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?before={before}&with_member={guildUsers}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuildScheduledEventUser>>()
                .Select(u => new GuildScheduledEventUser(u, guildId, this)).Reverse();
}