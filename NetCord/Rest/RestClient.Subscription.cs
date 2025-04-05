namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(Sku)], nameof(Sku.Id))]
    public IAsyncEnumerable<Subscription> GetSkuSubscriptionsAsync(ulong skuId, SubscriptionPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.Before, 100);

        var userId = paginationProperties.UserId;

        return new QueryPaginationAsyncEnumerable<Subscription, ulong>(
            this,
            paginationProperties,
            async s =>
            {
                var jsonSubscriptions = await s.ToObjectAsync(Serialization.Default.JsonSubscriptionArray).ConfigureAwait(false);
                return jsonSubscriptions.Select(e => new Subscription(e));
            },
            e => e.Id,
            HttpMethod.Get,
            $"/skus/{skuId}/subscriptions",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), userId.HasValue ? $"?user_id={userId}" : "?"),
            null,
            properties);
    }

    [GenerateAlias([typeof(Sku)], nameof(Sku.Id))]
    public async Task<Subscription> GetSkuSubscriptionAsync(ulong skuId, ulong subscriptionId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        return new(await (await SendRequestAsync(HttpMethod.Get, $"/skus/{skuId}/subscriptions/{subscriptionId}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSubscription).ConfigureAwait(false));
    }
}
