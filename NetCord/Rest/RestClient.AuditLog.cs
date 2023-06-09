using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async IAsyncEnumerable<RestAuditLogEntry> GetGuildAuditLogAsync(ulong guildId, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        Func<string> query;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}";
            else
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}";
        }
        else
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&action_type={actionType.GetValueOrDefault()}";
            else
                query = () => $"?limit=100";
        }

        JsonAuditLog? jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs", query(), new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);
        RestAuditLogEntryData data = new(jsonAuditLog, this);
        var entries = jsonAuditLog.AuditLogEntries;

        foreach (var jsonAuditLogEntry in entries)
            yield return new(jsonAuditLogEntry, data);

        if (entries.Length == 100)
        {
            await foreach (var auditLogEntry in GetGuildAuditLogBeforeAsync(guildId, entries[^1].Id))
                yield return auditLogEntry;
        }
    }

    public async IAsyncEnumerable<RestAuditLogEntry> GetGuildAuditLogBeforeAsync(ulong guildId, ulong before, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        Func<string> query;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&before={before}";
            else
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}&before={before}";
        }
        else
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&action_type={actionType.GetValueOrDefault()}&before={before}";
            else
                query = () => $"?limit=100&before={before}";
        }

        while (true)
        {
            var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs", query(), new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);
            RestAuditLogEntryData data = new(jsonAuditLog, this);
            var entries = jsonAuditLog.AuditLogEntries;

            foreach (var jsonAuditLogEntry in entries)
                yield return new(jsonAuditLogEntry, data);

            if (entries.Length != 100)
                break;

            before = entries[^1].Id;
        }
    }

    public async IAsyncEnumerable<RestAuditLogEntry> GetGuildAuditLogAfterAsync(ulong guildId, ulong after, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        Func<string> query;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&after={after}";
            else
                query = () => $"?limit=100&user_id={userId.GetValueOrDefault()}&after={after}";
        }
        else
        {
            if (actionType.HasValue)
                query = () => $"?limit=100&action_type={actionType.GetValueOrDefault()}&after={after}";
            else
                query = () => $"?limit=100&after={after}";
        }

        while (true)
        {
            var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs", query(), new(guildId), properties).ConfigureAwait(false)).ToObjectAsync(JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);
            RestAuditLogEntryData data = new(jsonAuditLog, this);
            var entries = jsonAuditLog.AuditLogEntries;

            foreach (var jsonAuditLogEntry in entries)
                yield return new(jsonAuditLogEntry, data);

            if (entries.Length != 100)
                break;

            after = entries[^1].Id;
        }
    }
}
