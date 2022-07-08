namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuildInvite> GetGuildInviteAsync(string inviteCode, bool withCounts = false, bool withExpiration = false, Snowflake? guildScheduledEventId = null, RequestProperties? properties = null)
    {
        if (guildScheduledEventId.HasValue)
            return new((await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}?with_counts={withCounts}&with_expiration={withExpiration}&guild_scheduled_event_id={guildScheduledEventId}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildInvite), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
        else
            return new((await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}?with_counts={withCounts}&with_expiration={withExpiration}", new RateLimits.Route(RateLimits.RouteParameter.GetGuildInvite), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
    }

    public async Task<RestGuildInvite> DeleteGuildInviteAsync(string inviteCode, RequestProperties? properties = null)
        => new((await SendRequestAsync(HttpMethod.Delete, $"/invites/{inviteCode}", new RateLimits.Route(RateLimits.RouteParameter.DeleteGuildInvite), properties).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
}