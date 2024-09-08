using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord.Rest;

public class RestAuditLogEntry(JsonAuditLogEntry jsonModel, RestAuditLogEntryData data, ulong guildId) : AuditLogEntry(jsonModel, guildId)
{
    /// <summary>
    /// Data of objects referenced in the audit log.
    /// </summary>
    public RestAuditLogEntryData Data { get; } = data;

    /// <summary>
    /// User that made the changes.
    /// </summary>
    public User? User
    {
        get
        {
            var userId = UserId;
            return userId.HasValue ? Data.Users[userId.GetValueOrDefault()] : null;
        }
    }
}
