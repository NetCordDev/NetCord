namespace NetCord.Rest;

public partial record OptionalGuildUsersPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, OptionalGuildUsersPaginationProperties>
{
    public bool WithGuildUsers { get; set; }

    static OptionalGuildUsersPaginationProperties IPaginationProperties<ulong, OptionalGuildUsersPaginationProperties>.Create() => new();
    static OptionalGuildUsersPaginationProperties IPaginationProperties<ulong, OptionalGuildUsersPaginationProperties>.Create(OptionalGuildUsersPaginationProperties properties) => new(properties);
}
