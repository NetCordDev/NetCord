namespace NetCord.Rest;

public partial record EntitlementsPaginationProperties : PaginationProperties<ulong>
{
    public ulong? UserId { get; set; }
    public IEnumerable<ulong>? SkuIds { get; set; }
    public ulong? GuildId { get; set; }
    public bool? ExcludeEnded { get; set; }
}
