namespace NetCord.Rest;

public partial class RestClient
{
    public async IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogAsync(Snowflake guildId, Snowflake? userId = null, AuditLogEvent? actionType = null, RequestProperties? options = null)
    {
        Task<Stream> task;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}", options);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}", options);
        }
        else
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&action_type={actionType.GetValueOrDefault()}", options);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100", options);
        }
        JsonModels.JsonAuditLog? jsonAuditLog = (await task.ConfigureAwait(false)).ToObject<JsonModels.JsonAuditLog>();

        foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
            yield return new(jsonAuditLogEntry, jsonAuditLog, this);

        await foreach (var auditLogEntry in GetGuildAuditLogBeforeAsync(guildId, jsonAuditLog.AuditLogEntries[^1].Id))
            yield return auditLogEntry;
    }

    public async IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogBeforeAsync(Snowflake guildId, Snowflake before, Snowflake? userId = null, AuditLogEvent? actionType = null, RequestProperties? options = null)
    {
        Task<Stream> task;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&before={before}", options);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&before={before}", options);
        }
        else
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&action_type={actionType.GetValueOrDefault()}&before={before}", options);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&before={before}", options);
        }
        JsonModels.JsonAuditLog? jsonAuditLog = (await task.ConfigureAwait(false)).ToObject<JsonModels.JsonAuditLog>();

        foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
            yield return new(jsonAuditLogEntry, jsonAuditLog, this);
    }
}