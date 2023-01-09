namespace NetCord.Rest;

public partial class RestClient
{
    public async IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogAsync(ulong guildId, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        Task<Stream> task;
        if (userId.HasValue)
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}", properties);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}", properties);
        }
        else
        {
            if (actionType.HasValue)
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&action_type={actionType.GetValueOrDefault()}", properties);
            else
                task = SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100", properties);
        }
        JsonModels.JsonAuditLog? jsonAuditLog = await (await task.ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

        foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
            yield return new(jsonAuditLogEntry, jsonAuditLog, this);

        await foreach (var auditLogEntry in GetGuildAuditLogBeforeAsync(guildId, jsonAuditLog.AuditLogEntries[^1].Id))
            yield return auditLogEntry;
    }

    public async IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogBeforeAsync(ulong guildId, ulong before, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        if (userId.HasValue)
        {
            if (actionType.HasValue)
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&before={before}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        before = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
            else
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&before={before}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        before = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
        }
        else
        {
            if (actionType.HasValue)
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&action_type={actionType.GetValueOrDefault()}&before={before}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        before = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
            else
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&before={before}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        before = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
        }
    }

    public async IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogAfterAsync(ulong guildId, ulong after, ulong? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null)
    {
        if (userId.HasValue)
        {
            if (actionType.HasValue)
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&action_type={actionType.GetValueOrDefault()}&after={after}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        after = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
            else
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&user_id={userId.GetValueOrDefault()}&after={after}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        after = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
        }
        else
        {
            if (actionType.HasValue)
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&action_type={actionType.GetValueOrDefault()}&after={after}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        after = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
            else
            {
                byte count;
                do
                {
                    count = 0;
                    var jsonAuditLog = await (await SendRequestAsync(HttpMethod.Get, $"/guilds/{guildId}/audit-logs?limit=100&after={after}", properties).ConfigureAwait(false)).ToObjectAsync(JsonModels.JsonAuditLog.JsonAuditLogSerializerContext.WithOptions.JsonAuditLog).ConfigureAwait(false);

                    foreach (var jsonAuditLogEntry in jsonAuditLog.AuditLogEntries)
                    {
                        yield return new(jsonAuditLogEntry, jsonAuditLog, this);
                        after = jsonAuditLogEntry.Id;
                        count++;
                    }
                }
                while (count == 100);
            }
        }
    }
}
