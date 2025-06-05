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
    /// The maximum number of items to retrieve in a single request.
    /// </summary>
    public int? BatchSize { get; set; }

    protected static abstract TSelf Create();

    protected static abstract TSelf Create(TSelf properties);
}
