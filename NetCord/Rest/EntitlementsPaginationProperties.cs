namespace NetCord.Rest;

public partial record EntitlementsPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, EntitlementsPaginationProperties>
{
    public ulong? UserId { get; set; }
    public IEnumerable<ulong>? SkuIds { get; set; }
    public ulong? GuildId { get; set; }
    public bool? ExcludeEnded { get; set; }

    static EntitlementsPaginationProperties IPaginationProperties<ulong, EntitlementsPaginationProperties>.Create() => new();
    static EntitlementsPaginationProperties IPaginationProperties<ulong, EntitlementsPaginationProperties>.Create(EntitlementsPaginationProperties properties) => new(properties);
}
