namespace NetCord.Rest;

internal class ContentPaginationAsyncEnumerable<T, TProperties, TFrom>(
    RestClient client,
    TProperties paginationProperties,
    Func<Stream, Task<IEnumerable<T>>> convertAsync,
    Func<T, TFrom> getId,
    HttpMethod method,
    FormattableString endpoint,
    PaginationContentBuilder<TProperties, TFrom> contentBuilder,
    TopLevelResourceInfo? resourceInfo,
    RequestProperties? properties,
    bool global = true) : IAsyncEnumerable<T> where TProperties : PaginationProperties<TFrom> where TFrom : struct
{
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var content = contentBuilder.Build(paginationProperties.From);

        var expectedCount = paginationProperties.Limit.GetValueOrDefault();

        while (true)
        {
            var results = await convertAsync(await client.SendRequestAsync(method, content, endpoint, null, resourceInfo, properties, global).ConfigureAwait(false)).ConfigureAwait(false);

            T? last = default;
            int count = 0;
            foreach (var result in results)
            {
                yield return last = result;
                count++;
            }

            if (count != expectedCount)
                yield break;

            content = contentBuilder.Build(getId(last!));
        }
    }
}
