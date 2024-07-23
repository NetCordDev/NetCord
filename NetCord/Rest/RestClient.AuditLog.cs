using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestGuild)], nameof(RestGuild.Id), TypeNameOverride = nameof(Guild))]
    public IAsyncEnumerable<RestAuditLogEntry> GetGuildAuditLogAsync(ulong guildId, GuildAuditLogPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.Before, 100);

        var userId = paginationProperties.UserId;
        var actionType = paginationProperties.ActionType;

        return new QueryPaginationAsyncEnumerable<RestAuditLogEntry, ulong>(
            this,
            paginationProperties,
            async s =>
            {
                var jsonAuditLog = await s.ToObjectAsync(Serialization.Default.JsonAuditLog).ConfigureAwait(false);
                RestAuditLogEntryData data = new(jsonAuditLog, this);
                return jsonAuditLog.AuditLogEntries.Select(e => new RestAuditLogEntry(e, data, guildId));
            },
            e => e.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/audit-logs",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), userId.HasValue
                                                                                                                                            ? (actionType.HasValue
                                                                                                                                                ? $"?user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&"
                                                                                                                                                : $"?user_id={userId.GetValueOrDefault()}&")
                                                                                                                                            : (actionType.HasValue
                                                                                                                                                ? $"?action_type={actionType.GetValueOrDefault()}&"
                                                                                                                                                : "?")),
            new(guildId),
            properties);
    }
}
