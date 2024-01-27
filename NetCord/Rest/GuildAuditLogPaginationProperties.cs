namespace NetCord.Rest;

public partial record GuildAuditLogPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, GuildAuditLogPaginationProperties>
{
    public ulong? UserId { get; set; }
    public AuditLogEvent? ActionType { get; set; }

    static GuildAuditLogPaginationProperties IPaginationProperties<ulong, GuildAuditLogPaginationProperties>.Create() => new();
    static GuildAuditLogPaginationProperties IPaginationProperties<ulong, GuildAuditLogPaginationProperties>.Create(GuildAuditLogPaginationProperties properties) => new(properties);
}
