using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<ulong, GuildScheduledEvent>> GetGuildScheduledEventsAsync(ulong guildId, bool withUserCount = false, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events?with_user_count={withUserCount}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildScheduledEvents), properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildScheduledEvent.JsonGuildScheduledEventArraySerializerContext.WithOptions.JsonGuildScheduledEventArray).ConfigureAwait(false)).ToDictionary(e => e.Id, e => new GuildScheduledEvent(e, this));

    public async Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(ulong guildId, GuildScheduledEventProperties guildScheduledEventProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildScheduledEventProperties>(guildScheduledEventProperties, GuildScheduledEventProperties.GuildScheduledEventPropertiesSerializerContext.WithOptions.GuildScheduledEventProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, $"/guilds/{guildId}/scheduled-events", new(RateLimits.RouteParameter.CreateGuildScheduledEvent), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent).ConfigureAwait(false), this);
    }

    public async Task<GuildScheduledEvent> GetGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, bool withUserCount = false, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}?with_user_count={withUserCount}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildScheduledEvent), properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent).ConfigureAwait(false), this);

    public async Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, Action<GuildScheduledEventOptions> action, RequestProperties? properties = null)
    {
        GuildScheduledEventOptions options = new();
        action(options);
        using (HttpContent content = new JsonContent<GuildScheduledEventOptions>(options, GuildScheduledEventOptions.GuildScheduledEventOptionsSerializerContext.WithOptions.GuildScheduledEventOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", new(RateLimits.RouteParameter.ModifyGuildScheduledEvent), content, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildScheduledEvent.JsonGuildScheduledEventSerializerContext.WithOptions.JsonGuildScheduledEvent).ConfigureAwait(false), this);
    }

    public Task DeleteGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildScheduledEvent), properties);

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAsync(ulong guildId, ulong scheduledEventId, bool guildUsers = false, RequestProperties? properties = null)
    {
        byte count = 0;
        GuildScheduledEventUser? last = null;
        foreach (var user in await GetMaxGuildScheduledEventUsersAsyncTask(guildId, scheduledEventId, guildUsers, properties).ConfigureAwait(false))
        {
            yield return last = user;
            count++;
        }
        if (count == 100)
        {
            await foreach (var user in GetGuildScheduledEventUsersAfterAsync(guildId, scheduledEventId, last!.User.Id, guildUsers, properties))
                yield return user;
        }
    }

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAfterAsync(ulong guildId, ulong scheduledEventId, ulong userId, bool guildUsers = false, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildScheduledEventUsersAfterAsyncTask(guildId, scheduledEventId, userId, guildUsers, properties).ConfigureAwait(false))
            {
                yield return user;
                userId = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersBeforeAsync(ulong guildId, ulong scheduledEventId, ulong userId, bool guildUsers = false, RequestProperties? properties = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in await GetMaxGuildScheduledEventUsersBeforeAsyncTask(guildId, scheduledEventId, userId, guildUsers, properties).ConfigureAwait(false))
            {
                yield return user;
                userId = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersAsyncTask(ulong guildId, ulong scheduledEventId, bool guildUsers, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?with_member={guildUsers}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildScheduledEventUsers), properties).ConfigureAwait(false))
                .ToObjectAsync(JsonGuildScheduledEventUser.JsonGuildScheduledEventUserArraySerializerContext.WithOptions.JsonGuildScheduledEventUserArray).ConfigureAwait(false))
                .Select(u => new GuildScheduledEventUser(u, guildId, this));

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersAfterAsyncTask(ulong guildId, ulong scheduledEventId, ulong after, bool guildUsers, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?after={after}&with_member={guildUsers}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildScheduledEventUsers), properties).ConfigureAwait(false))
                .ToObjectAsync(JsonGuildScheduledEventUser.JsonGuildScheduledEventUserArraySerializerContext.WithOptions.JsonGuildScheduledEventUserArray).ConfigureAwait(false))
                .Select(u => new GuildScheduledEventUser(u, guildId, this));

    private async Task<IEnumerable<GuildScheduledEventUser>> GetMaxGuildScheduledEventUsersBeforeAsyncTask(ulong guildId, ulong scheduledEventId, ulong before, bool guildUsers, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?before={before}&with_member={guildUsers}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildScheduledEventUsers), properties).ConfigureAwait(false))
                .ToObjectAsync(JsonGuildScheduledEventUser.JsonGuildScheduledEventUserArraySerializerContext.WithOptions.JsonGuildScheduledEventUserArray).ConfigureAwait(false))
                .Select(u => new GuildScheduledEventUser(u, guildId, this)).Reverse();
}
