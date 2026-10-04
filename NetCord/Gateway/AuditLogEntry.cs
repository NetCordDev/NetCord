using NetCord.JsonModels;

namespace NetCord.Gateway;

public class AuditLogEntry(JsonAuditLogEntry jsonModel, ulong guildId) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// ID of the affected entity.
    /// </summary>
    public ulong? TargetId { get; } = jsonModel.TargetId;

    /// <summary>
    /// Changes made to the <see cref="TargetId"/>.
    /// </summary>
    public IReadOnlyDictionary<string, AuditLogChange> Changes { get; } = jsonModel.Changes.ToDictionaryOrEmpty(c => c.Key, c => new AuditLogChange(c));

    /// <summary>
    /// ID of user that made the changes.
    /// </summary>
    public ulong? UserId { get; } = jsonModel.UserId;

    /// <summary>
    /// Type of action that occurred.
    /// </summary>
    public AuditLogEvent ActionType { get; } = jsonModel.ActionType;

    /// <summary>
    /// Additional info for certain event types.
    /// </summary>
    public AuditLogEntryInfo? Options { get; } = jsonModel.Options is { } options ? new(options) : null;

    /// <summary>
    /// Reason for the change (1-512 characters).
    /// </summary>
    public string? Reason { get; } = jsonModel.Reason;

    /// <summary>
    /// The ID of the guild this audit log entry belongs to.
    /// </summary>
    public ulong GuildId { get; } = guildId;
}
