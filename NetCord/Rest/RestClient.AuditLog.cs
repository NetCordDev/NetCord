using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public IAsyncEnumerable<RestAuditLogEntry> GetGuildAuditLogAsync(ulong guildId, GuildAuditLogPaginationProperties? guildAuditLogPaginationProperties = null, RequestProperties? properties = null)
    {
        ulong? userId;
        AuditLogEvent? actionType;
        if (guildAuditLogPaginationProperties is not null)
        {
            userId = guildAuditLogPaginationProperties.UserId;
            actionType = guildAuditLogPaginationProperties.ActionType;
        }
        else
        {
            userId = null;
            actionType = null;
        }

        var paginationProperties = PaginationProperties<ulong>.Prepare(guildAuditLogPaginationProperties, 0, long.MaxValue, PaginationDirection.Before, 100);

        return new PaginationAsyncEnumerable<RestAuditLogEntry, ulong>(
            this,
            paginationProperties,
            async s =>
            {
                var jsonAuditLog = await s.ToObjectAsync(JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);
                RestAuditLogEntryData data = new(jsonAuditLog, this);
                return jsonAuditLog.AuditLogEntries.Select(e => new RestAuditLogEntry(e, data));
            },
            e => e.Id,
            HttpMethod.Get,
            $"/guilds/{guildId}/audit-logs",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString(), userId.HasValue
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
