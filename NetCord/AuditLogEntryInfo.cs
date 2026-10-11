using NetCord.JsonModels;

namespace NetCord;

public class AuditLogEntryInfo(JsonAuditLogEntryInfo jsonModel)
{
    /// <summary>
    /// ID of the application whose permissions were targeted.
    /// </summary>
    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// Name of the Auto Moderation rule that was triggered.
    /// </summary>
    public string? AutoModerationRuleName { get; } = jsonModel.AutoModerationRuleName;

    /// <summary>
    /// Trigger type of the Auto Moderation rule that was triggered.
    /// </summary>
    public AutoModerationRuleTriggerType? AutoModerationRuleTriggerType { get; } = (AutoModerationRuleTriggerType?)jsonModel.AutoModerationRuleTriggerType;

    /// <summary>
    /// Channel in which the entities were targeted.
    /// </summary>
    public ulong? ChannelId { get; } = jsonModel.ChannelId;

    /// <summary>
    /// Number of entities that were targeted.
    /// </summary>
    public int? Count { get; } = jsonModel.Count;

    /// <summary>
    /// Number of days after which inactive members were kicked.
    /// </summary>
    public int? DeleteGuildUserDays { get; } = jsonModel.DeleteGuildUserDays;

    /// <summary>
    /// ID of the overwritten entity.
    /// </summary>
    public ulong? Id { get; } = jsonModel.Id;

    /// <summary>
    /// Number of members removed by the prune.
    /// </summary>
    public int? GuildUsersRemoved { get; } = jsonModel.GuildUsersRemoved;

    /// <summary>
    /// ID of the message that was targeted.
    /// </summary>
    public ulong? MessageId { get; } = jsonModel.MessageId;

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string? RoleName { get; } = jsonModel.RoleName;

    /// <summary>
    /// Type of overwritten entity.
    /// </summary>
    public PermissionOverwriteType? Type { get; } = (PermissionOverwriteType?)jsonModel.Type;

    /// <summary>
    /// Type of integration which performed the action.
    /// </summary>
    public IntegrationType? IntegrationType { get; } = jsonModel.IntegrationType;

    /// <summary>
    /// The new voice channel status.
    /// </summary>
    public string? Status { get; } = jsonModel.Status;
}
