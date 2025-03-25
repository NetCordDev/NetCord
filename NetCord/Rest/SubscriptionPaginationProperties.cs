namespace NetCord.Rest;

public partial record SubscriptionPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, SubscriptionPaginationProperties>
{
    /// <summary>
    /// The ID of the user for which to return subscriptions. Required except for OAuth queries.
    /// </summary>
    public ulong? UserId { get; set; }

    public static SubscriptionPaginationProperties Create() => new();
    public static SubscriptionPaginationProperties Create(SubscriptionPaginationProperties properties) => new(properties);
}
