using NetCord.JsonModels;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<CurrentUser> GetCurrentUserAsync(RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);

    public async Task<User> GetUserAsync(ulong userId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/{userId}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);

    public async Task<CurrentUser> ModifyCurrentUserAsync(Action<CurrentUserOptions> action, RequestProperties? properties = null)
    {
        CurrentUserOptions CurrentUserOptions = new();
        action(CurrentUserOptions);
        using (HttpContent content = new JsonContent<CurrentUserOptions>(CurrentUserOptions, CurrentUserOptions.CurrentUserOptionsSerializerContext.WithOptions.CurrentUserOptions))
            return new(await (await SendRequestAsync(HttpMethod.Patch, content, $"/users/@me", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonUser.JsonUserSerializerContext.WithOptions.JsonUser).ConfigureAwait(false), this);
    }

    public IAsyncEnumerable<RestGuild> GetCurrentUserGuildsAsync(GuildsPaginationProperties? guildsPaginationProperties = null, RequestProperties? properties = null)
    {
        var withCounts = guildsPaginationProperties is not null && guildsPaginationProperties.WithCounts;

        var paginationProperties = PaginationProperties<ulong>.Prepare(guildsPaginationProperties, 0, long.MaxValue, PaginationDirection.After, 200);

        return new PaginationAsyncEnumerable<RestGuild, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(JsonGuild.JsonGuildArraySerializerContext.WithOptions.JsonGuildArray).ConfigureAwait(false)).Select(g => new RestGuild(g, this)),
            g => g.Id,
            HttpMethod.Get,
            $"/users/@me/guilds",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString(), $"?with_counts={withCounts}&"),
            null,
            properties);
    }

    public async Task<GuildUser> GetCurrentUserGuildUserAsync(ulong guildId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/guilds/{guildId}/member", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonGuildUser.JsonGuildUserSerializerContext.WithOptions.JsonGuildUser).ConfigureAwait(false), guildId, this);

    public Task LeaveGuildAsync(ulong guildId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/users/@me/guilds/{guildId}", null, null, properties);

    public async Task<DMChannel> GetDMChannelAsync(ulong userId, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<DMChannelProperties>(new(userId), DMChannelProperties.DMChannelPropertiesSerializerContext.WithOptions.DMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/users/@me/channels", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<GroupDMChannel> CreateGroupDMChannelAsync(GroupDMChannelProperties groupDMChannelProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<GroupDMChannelProperties>(groupDMChannelProperties, GroupDMChannelProperties.GroupDMChannelPropertiesSerializerContext.WithOptions.GroupDMChannelProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/users/@me/channels", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonChannel.JsonChannelSerializerContext.WithOptions.JsonChannel).ConfigureAwait(false), this);
    }

    public async Task<IReadOnlyDictionary<ulong, Connection>> GetCurrentUserConnectionsAsync(RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/connections", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonConnection.JsonConnectionArraySerializerContext.WithOptions.JsonConnectionArray).ConfigureAwait(false)).ToDictionary(c => c.Id, c => new Connection(c, this));

    public async Task<ApplicationRoleConnection> GetCurrentUserApplicationRoleConnectionAsync(ulong applicationId, RequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/users/@me/applications/{applicationId}/role-connection", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationRoleConnection.JsonApplicationRoleConnectionSerializerContext.WithOptions.JsonApplicationRoleConnection).ConfigureAwait(false));

    public async Task<ApplicationRoleConnection> UpdateCurrentUserApplicationRoleConnectionAsync(ulong applicationId, ApplicationRoleConnectionProperties applicationRoleConnectionProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<ApplicationRoleConnectionProperties>(applicationRoleConnectionProperties, ApplicationRoleConnectionProperties.ApplicationRoleConnectionPropertiesSerializerContext.WithOptions.ApplicationRoleConnectionProperties))
            return new(await (await SendRequestAsync(HttpMethod.Put, content, $"/users/@me/applications/{applicationId}/role-connection", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonApplicationRoleConnection.JsonApplicationRoleConnectionSerializerContext.WithOptions.JsonApplicationRoleConnection).ConfigureAwait(false));
    }
}
