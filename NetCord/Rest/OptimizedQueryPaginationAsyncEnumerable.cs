namespace NetCord.Rest;

internal class OptimizedQueryPaginationAsyncEnumerable<T, TFrom>(
    RestClient client,
    PaginationProperties<TFrom> paginationProperties,
    Func<Stream, Task<(IEnumerable<T> Results, bool HasMore)>> convertAsync,
    Func<T, TFrom> getId,
    HttpMethod method,
    FormattableString endpoint,
    PaginationQueryBuilder<TFrom> queryBuilder,
    TopLevelResourceInfo? resourceInfo,
    RequestProperties? properties,
    bool global = true) : IAsyncEnumerable<T> where TFrom : struct
{
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var query = queryBuilder.ToString(paginationProperties.From);

        while (true)
        {
            (var results, var hasMore) = await convertAsync(await client.SendRequestAsync(method, endpoint, query, resourceInfo, properties, global).ConfigureAwait(false)).ConfigureAwait(false);

            T? last = default;

            foreach (var result in results)
                yield return last = result;

            if (!hasMore)
                yield break;

            query = queryBuilder.ToString(getId(last!));
        }
    }
}
