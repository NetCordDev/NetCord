namespace NetCord.Rest;

public partial record GuildsPaginationProperties : PaginationProperties<ulong>
{
    public bool WithCounts { get; set; }
}
