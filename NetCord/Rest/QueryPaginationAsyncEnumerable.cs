namespace NetCord.Rest;

internal class QueryPaginationAsyncEnumerable<T, TFrom>(
    RestClient client,
    PaginationProperties<TFrom> paginationProperties,
    Func<Stream, Task<IEnumerable<T>>> convertAsync,
    Func<T, TFrom> getId,
    HttpMethod method,
    FormattableString endpoint,
    PaginationQueryBuilder<TFrom> queryBuilder,
    TopLevelResourceInfo? resourceInfo,
    RestRequestProperties? properties,
    bool global = true) : IAsyncEnumerable<T> where TFrom : struct
{
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var query = queryBuilder.ToString(paginationProperties.From);

        var expectedCount = paginationProperties.Limit.GetValueOrDefault();

        while (true)
        {
            var results = await convertAsync(await client.SendRequestAsync(method, endpoint, query, resourceInfo, properties, global, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            T? last = default;
            int count = 0;
            foreach (var result in results)
            {
                yield return last = result;
                count++;
            }

            if (count != expectedCount)
                yield break;

            query = queryBuilder.ToString(getId(last!));
        }
    }
}
