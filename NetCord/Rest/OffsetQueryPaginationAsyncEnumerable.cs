using System.Numerics;

namespace NetCord.Rest;

internal class OffsetQueryPaginationAsyncEnumerable<T, TFrom>(
    RestClient client,
    PaginationProperties<TFrom> paginationProperties,
    Func<Stream, Task<(IEnumerable<T> Results, bool Retry)>> convertAsync,
    HttpMethod method,
    FormattableString endpoint,
    PaginationQueryBuilder<TFrom> queryBuilder,
    TopLevelResourceInfo? resourceInfo,
    RestRequestProperties? properties,
    bool global = true) : IAsyncEnumerable<T> where TFrom : struct, IBinaryInteger<TFrom>
{
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var from = paginationProperties.From;

        var query = queryBuilder.ToString(from);

        var offset = from.GetValueOrDefault();

        var expectedCount = paginationProperties.BatchSize.GetValueOrDefault();

        var increment = TFrom.CreateChecked(expectedCount);

        while (true)
        {
            var (results, retry) = await convertAsync(await client.SendRequestAsync(method, endpoint, query, resourceInfo, properties, global, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            int count = 0;
            foreach (var result in results)
            {
                yield return result;
                count++;
            }

            if (!retry)
            {
                if (count != expectedCount)
                    yield break;

                query = queryBuilder.ToString(offset += increment);
            }
        }
    }
}
