using System.Text;

namespace NetCord.Rest;

public partial class RestClient
{
    public IAsyncEnumerable<Entitlement> GetEntitlementsAsync(ulong applicationId, EntitlementsPaginationProperties? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.Prepare(paginationProperties, 0, long.MaxValue, PaginationDirection.After, 100);

        var userId = paginationProperties.UserId;
        var skuIds = paginationProperties.SkuIds;
        var guildId = paginationProperties.GuildId;
        var excludeEnded = paginationProperties.ExcludeEnded;

        string baseQuery;
        StringBuilder stringBuilder = new("?");

        if (userId.HasValue)
            stringBuilder.Append("user_id=").Append(userId.GetValueOrDefault()).Append('&');
        if (skuIds is not null)
            stringBuilder.Append("sku_ids=").AppendJoin(',', skuIds).Append('&');
        if (guildId.HasValue)
            stringBuilder.Append("guild_id=").Append(guildId.GetValueOrDefault()).Append('&');
        if (excludeEnded.HasValue)
            stringBuilder.Append("exclude_ended=").Append(excludeEnded.GetValueOrDefault()).Append('&');

        baseQuery = stringBuilder.ToString();

        return new QueryPaginationAsyncEnumerable<Entitlement, ulong>(
            this,
            paginationProperties,
            paginationProperties.Direction.GetValueOrDefault() switch
            {
                PaginationDirection.After => async s => (await s.ToObjectAsync(Serialization.Default.JsonEntitlementArray).ConfigureAwait(false)).Select(e => new Entitlement(e)),
                PaginationDirection.Before => async s => (await s.ToObjectAsync(Serialization.Default.JsonEntitlementArray).ConfigureAwait(false)).GetReversedIEnumerable().Select(e => new Entitlement(e)),
                _ => throw new ArgumentException($"The value of '{nameof(paginationProperties)}.{nameof(paginationProperties.Direction)}' is invalid.", nameof(paginationProperties)),
            },
            e => e.Id,
            HttpMethod.Get,
            $"/applications/{applicationId}/entitlements",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString(), baseQuery),
            null,
            properties);
    }

    public Task ConsumeEntitlementAsync(ulong applicationId, ulong entitlementId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Post, $"/applications/{applicationId}/entitlements/{entitlementId}/consume", null, null, properties, cancellationToken: cancellationToken);

    public async Task<Entitlement> CreateTestEntitlementAsync(ulong applicationId, TestEntitlementProperties testEntitlementProperties, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new JsonContent<TestEntitlementProperties>(testEntitlementProperties, Serialization.Default.TestEntitlementProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/applications/{applicationId}/entitlements", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonEntitlement).ConfigureAwait(false));
    }

    public Task DeleteTestEntitlementAsync(ulong applicationId, ulong entitlementId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/entitlements/{entitlementId}", null, null, properties, cancellationToken: cancellationToken);
}
