namespace NetCord.Rest;

internal class OptimizedPaginationAsyncEnumerable<T, TFrom> : IAsyncEnumerable<T> where TFrom : struct
{
    public OptimizedPaginationAsyncEnumerable(
        RestClient client,
        PaginationProperties<TFrom> paginationProperties,
        Func<Stream, Task<(IEnumerable<T> Results, bool HasMore)>> convertAsync,
        Func<T, TFrom> getId,
        HttpMethod method,
        FormattableString endpoint,
        PaginationQueryBuilder<TFrom> queryBuilder,
        TopLevelResourceInfo? resourceInfo,
        RequestProperties? properties,
        bool global = true)
    {
        _client = client;
        _paginationProperties = paginationProperties;
        _convertAsync = convertAsync;
        _getId = getId;
        _method = method;
        _endpoint = endpoint;
        _queryBuilder = queryBuilder;
        _resourceInfo = resourceInfo;
        _properties = properties;
        _global = global;
    }

    private readonly RestClient _client;
    private readonly PaginationProperties<TFrom> _paginationProperties;
    private readonly Func<Stream, Task<(IEnumerable<T> Results, bool HasMore)>> _convertAsync;
    private readonly Func<T, TFrom> _getId;
    private readonly HttpMethod _method;
    private readonly FormattableString _endpoint;
    private readonly PaginationQueryBuilder<TFrom> _queryBuilder;
    private readonly TopLevelResourceInfo? _resourceInfo;
    private readonly RequestProperties? _properties;
    private readonly bool _global;

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var convertAsync = _convertAsync;
        var client = _client;
        var method = _method;
        var endpoint = _endpoint;

        var paginationProperties = _paginationProperties;
        var queryBuilder = _queryBuilder;
        var direction = paginationProperties.Direction.GetValueOrDefault();
        var query = queryBuilder.ToString(paginationProperties.From, direction);

        var resourceInfo = _resourceInfo;
        var properties = _properties;
        var global = _global;
        var expectedCount = paginationProperties.Limit.GetValueOrDefault();
        var getId = _getId;

        while (true)
        {
            (var results, var hasMore) = await convertAsync(await client.SendRequestAsync(method, endpoint, query, resourceInfo, properties, global).ConfigureAwait(false)).ConfigureAwait(false);

            T? last = default;

            foreach (var result in results)
                yield return last = result;

            if (!hasMore)
                yield break;

            query = queryBuilder.ToString(getId(last!), direction);
        }
    }
}
