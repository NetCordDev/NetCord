namespace NetCord;

public partial class RestClient
{
    public async Task<IReadOnlyDictionary<DiscordId, GuildScheduledEvent>> GetGuildScheduledEventsAsync(DiscordId guildId, bool withUserCount = false, RequestProperties? options = null)
        => (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events?with_user_count={withUserCount}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent[]>().ToDictionary(e => e.Id, e => new GuildScheduledEvent(e, this));

    public async Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(DiscordId guildId, GuildScheduledEventProperties properties, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Post, new JsonContent(properties), $"/guilds/{guildId}/scheduled-events", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);

    public async Task<GuildScheduledEvent> GetGuildScheduledEventAsync(DiscordId guildId, DiscordId scheduledEventId, bool withUserCount = false, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}?with_user_count={withUserCount}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);

    public async Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(DiscordId guildId, DiscordId scheduledEventId, Action<GuildScheduledEventOptions> action, RequestProperties? options = null)
    {
        GuildScheduledEventOptions o = new();
        action(o);
        return new((await SendRequestAsync(HttpMethod.Get, new JsonContent(o), $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", options).ConfigureAwait(false))!.ToObject<JsonModels.JsonGuildScheduledEvent>(), this);
    }

    public Task DeleteGuildScheduledEventAsync(DiscordId guildId, DiscordId scheduledEventId, RequestProperties? options = null)
        => SendRequestAsync(HttpMethod.Delete, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}", options);

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAfterAsync(DiscordId guildId, DiscordId scheduledEventId, DiscordId? after = null, bool guildUsers = false, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?after={(after.HasValue ? after.GetValueOrDefault() : "null")}&with_member={guildUsers}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuildScheduledEventUser>>()
                .Select(u => new GuildScheduledEventUser(u, guildId, this)))
            {
                yield return user;
                after = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }

    public async IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersBeforeAsync(DiscordId guildId, DiscordId scheduledEventId, DiscordId? before = null, bool guildUsers = false, RequestProperties? options = null)
    {
        byte count;
        do
        {
            count = 0;
            foreach (var user in (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/scheduled-events/{scheduledEventId}/users?before={(before.HasValue ? before.GetValueOrDefault() : "null")}&with_member={guildUsers}", options).ConfigureAwait(false))!
                .ToObject<IEnumerable<JsonModels.JsonGuildScheduledEventUser>>()
                .Select(u => new GuildScheduledEventUser(u, guildId, this)))
            {
                yield return user;
                before = user.User.Id;
                count++;
            }
        }
        while (count == 100);
    }
}