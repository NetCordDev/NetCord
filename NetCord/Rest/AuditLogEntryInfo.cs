using NetCord.JsonModels;

namespace NetCord.Rest;

public class AuditLogEntryInfo : IJsonModel<JsonAuditLogEntryInfo>
{
    JsonAuditLogEntryInfo IJsonModel<JsonAuditLogEntryInfo>.JsonModel => _jsonModel;
    private readonly JsonAuditLogEntryInfo _jsonModel;

    public AuditLogEntryInfo(JsonAuditLogEntryInfo jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// Id of the app whose permissions were targeted.
    /// </summary>
    public ulong? ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Name of the Auto Moderation rule that was triggered.
    /// </summary>
    public string? AutoModerationRuleName => _jsonModel.AutoModerationRuleName;

    /// <summary>
    /// Trigger type of the Auto Moderation rule that was triggered.
    /// </summary>
    public AutoModerationRuleTriggerType? AutoModerationRuleTriggerType => (AutoModerationRuleTriggerType?)_jsonModel.AutoModerationRuleTriggerType;

    /// <summary>
    /// Channel in which the entities were targeted.
    /// </summary>
    public ulong? ChannelId => _jsonModel.ChannelId;

    /// <summary>
    /// Number of entities that were targeted.
    /// </summary>
    public int? Count => _jsonModel.Count;

    /// <summary>
    /// Number of days after which inactive members were kicked.
    /// </summary>
    public int? DeleteGuildUserDays => _jsonModel.DeleteGuildUserDays;

    /// <summary>
    /// Id of the overwritten entity.
    /// </summary>
    public ulong? Id => _jsonModel.Id;

    /// <summary>
    /// Number of members removed by the prune.
    /// </summary>
    public int? GuildUsersRemoved => _jsonModel.GuildUsersRemoved;

    /// <summary>
    /// Id of the message that was targeted.
    /// </summary>
    public ulong? MessageId => _jsonModel.MessageId;

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string? RoleName => _jsonModel.RoleName;

    /// <summary>
    /// Type of overwritten entity.
    /// </summary>
    public PermissionOverwriteType? Type => (PermissionOverwriteType?)_jsonModel.Type;
}
