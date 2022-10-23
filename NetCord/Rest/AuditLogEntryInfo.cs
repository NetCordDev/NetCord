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

    public ulong? ApplicationId => _jsonModel.ApplicationId;

    public ulong? ChannelId => _jsonModel.ChannelId;

    public int? Count => _jsonModel.Count;

    public int? DeleteGuildUserDays => _jsonModel.DeleteGuildUserDays;

    public ulong? Id => _jsonModel.Id;

    public int? GuildUsersRemoved => _jsonModel.GuildUsersRemoved;

    public ulong? MessageId => _jsonModel.MessageId;

    public string? RoleName => _jsonModel.RoleName;

    public PermissionOverwriteType? Type => (PermissionOverwriteType?)_jsonModel.Type;
}
