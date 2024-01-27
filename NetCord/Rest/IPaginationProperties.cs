namespace NetCord.Rest;

internal interface IPaginationProperties<T, TSelf> where T : struct where TSelf : IPaginationProperties<T, TSelf>
{
    /// <summary>
    /// The starting point for pagination.
    /// </summary>
    public T? From { get; set; }

    /// <summary>
    /// The direction of pagination.
    /// </summary>
    public PaginationDirection? Direction { get; set; }

    /// <summary>
    /// The number of items to download at once.
    /// </summary>
    public int? Limit { get; set; }

    protected static abstract TSelf Create();

    protected static abstract TSelf Create(TSelf properties);
}
