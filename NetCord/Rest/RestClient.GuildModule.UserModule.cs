namespace NetCord;

public partial class RestClient
{
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