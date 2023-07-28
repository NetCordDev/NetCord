namespace NetCord.Rest;

public partial record OptionalGuildUsersPaginationProperties : PaginationProperties<ulong>
{
    public bool WithGuildUsers { get; set; }
}
