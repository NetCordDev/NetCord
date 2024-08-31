using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestInvite)], nameof(RestInvite.Code), TypeNameOverride = nameof(Invite))]
    public async Task<RestInvite> GetGuildInviteAsync(string inviteCode, bool withCounts = false, bool withExpiration = false, ulong? guildScheduledEventId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        if (guildScheduledEventId.HasValue)
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}&guild_scheduled_event_id={guildScheduledEventId}", null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
        else
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}", null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestInvite)], nameof(RestInvite.Code), TypeNameOverride = nameof(Invite))]
    public async Task<RestInvite> DeleteGuildInviteAsync(string inviteCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/invites/{inviteCode}", null, null, properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
}
