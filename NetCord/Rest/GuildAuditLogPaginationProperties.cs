namespace NetCord.Rest;

public partial record GuildAuditLogPaginationProperties : PaginationProperties<ulong>
{
    public ulong? UserId { get; set; }
    public AuditLogEvent? ActionType { get; set; }
}
