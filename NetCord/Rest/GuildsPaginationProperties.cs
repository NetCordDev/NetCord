namespace NetCord.Rest;

public partial record GuildsPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, GuildsPaginationProperties>
{
    public bool WithCounts { get; set; }

    static GuildsPaginationProperties IPaginationProperties<ulong, GuildsPaginationProperties>.Create() => new();
    static GuildsPaginationProperties IPaginationProperties<ulong, GuildsPaginationProperties>.Create(GuildsPaginationProperties properties) => new(properties);
}
