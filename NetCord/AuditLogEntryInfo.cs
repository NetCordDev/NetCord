using NetCord.JsonModels;

namespace NetCord;

public class AuditLogEntryInfo(JsonAuditLogEntryInfo jsonModel) : IJsonModel<JsonAuditLogEntryInfo>
{
    JsonAuditLogEntryInfo IJsonModel<JsonAuditLogEntryInfo>.JsonModel => jsonModel;

    /// <summary>
    /// ID of the app whose permissions were targeted.
    /// </summary>
    public ulong? ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// Name of the Auto Moderation rule that was triggered.
    /// </summary>
    public string? AutoModerationRuleName => jsonModel.AutoModerationRuleName;

    /// <summary>
    /// Trigger type of the Auto Moderation rule that was triggered.
    /// </summary>
    public AutoModerationRuleTriggerType? AutoModerationRuleTriggerType => (AutoModerationRuleTriggerType?)jsonModel.AutoModerationRuleTriggerType;

    /// <summary>
    /// Channel in which the entities were targeted.
    /// </summary>
    public ulong? ChannelId => jsonModel.ChannelId;

    /// <summary>
    /// Number of entities that were targeted.
    /// </summary>
    public int? Count => jsonModel.Count;

    /// <summary>
    /// Number of days after which inactive members were kicked.
    /// </summary>
    public int? DeleteGuildUserDays => jsonModel.DeleteGuildUserDays;

    /// <summary>
    /// ID of the overwritten entity.
    /// </summary>
    public ulong? Id => jsonModel.Id;

    /// <summary>
    /// Number of members removed by the prune.
    /// </summary>
    public int? GuildUsersRemoved => jsonModel.GuildUsersRemoved;

    /// <summary>
    /// ID of the message that was targeted.
    /// </summary>
    public ulong? MessageId => jsonModel.MessageId;

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string? RoleName => jsonModel.RoleName;

    /// <summary>
    /// Type of overwritten entity.
    /// </summary>
    public PermissionOverwriteType? Type => (PermissionOverwriteType?)jsonModel.Type;

    /// <summary>
    /// Type of integration which performed the action.
    /// </summary>
    public IntegrationType? IntegrationType => jsonModel.IntegrationType;
}
