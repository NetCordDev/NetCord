using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<IReadOnlyDictionary<ulong, GuildScheduledEvent>> GetGuildScheduledEventsAsync(ulong guildId, bool withUserCount = false, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events", $"?with_user_count={withUserCount}", new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildScheduledEventArray).ConfigureAwait(false)).ToDictionary(e => e.Id, e => new GuildScheduledEvent(e, this));

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public async Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(ulong guildId, GuildScheduledEventProperties guildScheduledEventProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GuildScheduledEventProperties>(guildScheduledEventProperties, Serialization.Default.GuildScheduledEventProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/guilds/{guildId}/scheduled-events", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildScheduledEvent).ConfigureAwait(false), this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildScheduledEvent), nameof(GuildScheduledEvent.GuildId), nameof(GuildScheduledEvent.Id))]
    public async Task<GuildScheduledEvent> GetGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, bool withUserCount = false, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", $"?with_user_count={withUserCount}", new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildScheduledEvent).ConfigureAwait(false), this);

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildScheduledEvent), nameof(GuildScheduledEvent.GuildId), nameof(GuildScheduledEvent.Id))]
    public async Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, Action<GuildScheduledEventOptions> action, RequestProperties? properties = null)
    {
        GuildScheduledEventOptions options = new();
        action(options);
        using (HttpContent content = new JsonContent<GuildScheduledEventOptions>(options, Serialization.Default.GuildScheduledEventOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", null, new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonGuildScheduledEvent).ConfigureAwait(false), this);
    }

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildScheduledEvent), nameof(GuildScheduledEvent.GuildId), nameof(GuildScheduledEvent.Id))]
    public Task DeleteGuildScheduledEventAsync(ulong guildId, ulong scheduledEventId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", null, new(guildId), properties);

    [GenerateAlias(typeof(RestGuild), nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    [GenerateAlias(typeof(GuildScheduledEvent), nameof(GuildScheduledEvent.GuildId), nameof(GuildScheduledEvent.Id))]
    public IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAsync(ulong guildId, ulong scheduledEventId, OptionalGuildUsersPaginationProperties? paginationProperties = null, RequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.After, 100);

        return new QueryPaginationAsyncEnumerable<GuildScheduledEventUser, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildScheduledEventUserArray).ConfigureAwait(false)).Select(u => new GuildScheduledEventUser(u, guildId, this)),
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonGuildScheduledEventUserArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(u => new GuildScheduledEventUser(u, guildId, this)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            u => u.User.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), $"?with_member={paginationProperties.WithGuildUsers}&"),
            new(guildId),
            properties);
    }
}
