using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestGuildInvite)], nameof(RestGuildInvite.Code), TypeNameOverride = nameof(GuildInvite))]
    public async Task<RestGuildInvite> GetGuildInviteAsync(string inviteCode, bool withCounts = false, bool withExpiration = false, ulong? guildScheduledEventId = null, RestRequestProperties? properties = null)
    {
        if (guildScheduledEventId.HasValue)
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}&guild_scheduled_event_id={guildScheduledEventId}", null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInvite).ConfigureAwait(false), this);
        else
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}", null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInvite).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestGuildInvite)], nameof(RestGuildInvite.Code), TypeNameOverride = nameof(GuildInvite))]
    public async Task<RestGuildInvite> DeleteGuildInviteAsync(string inviteCode, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/invites/{inviteCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestGuildInvite).ConfigureAwait(false), this);
}
