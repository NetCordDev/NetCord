namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<RestGuildInvite> GetGuildInviteAsync(string inviteCode, bool withCounts = false, bool withExpiration = false, Snowflake? guildScheduledEventId = null, RequestProperties? options = null)
    {
        if (guildScheduledEventId.HasValue)
            return new((await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}?with_counts={withCounts}&with_expiration={withExpiration}&guild_scheduled_event_id={guildScheduledEventId}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
        else
            return new((await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}?with_counts={withCounts}&with_expiration={withExpiration}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
    }

    public async Task<RestGuildInvite> DeleteGuildInviteAsync(string inviteCode, RequestProperties? options = null)
        => new((await SendRequestAsync(HttpMethod.Delete, $"/invites/{inviteCode}", options).ConfigureAwait(false)).ToObject<JsonModels.JsonRestGuildInvite>(), this);
}
