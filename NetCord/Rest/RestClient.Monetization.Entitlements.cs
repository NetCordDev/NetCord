using System.Text;

using NetCord.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public IAsyncEnumerable<Entitlement> GetEntitlementsAsync(ulong applicationId, EntitlementsPaginationProperties? entitlementsPaginationProperties = null, RequestProperties? properties = null)
    {
        var userId = entitlementsPaginationProperties?.UserId;
        var skuIds = entitlementsPaginationProperties?.SkuIds;
        var guildId = entitlementsPaginationProperties?.GuildId;
        var excludeEnded = entitlementsPaginationProperties?.ExcludeEnded;

        var paginationProperties = PaginationProperties<ulong>.Prepare(entitlementsPaginationProperties, 0, long.MaxValue, PaginationDirection.After, 100);

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

        return new PaginationAsyncEnumerable<Entitlement, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(JsonEntitlement.JsonEntitlementArraySerializerContext.WithOptions.JsonEntitlementArray).ConfigureAwait(false)).Select(e => new Entitlement(e)),
            e => e.Id,
            HttpMethod.Get,
            $"/applications/{applicationId}/entitlements",
            new(paginationProperties.Limit.GetValueOrDefault(), id => id.ToString(), baseQuery),
            null,
            properties);
    }

    public async Task<Entitlement> CreateTestEntitlementAsync(ulong applicationId, TestEntitlementProperties testEntitlementProperties, RequestProperties? properties = null)
    {
        using (HttpContent content = new JsonContent<TestEntitlementProperties>(testEntitlementProperties, TestEntitlementProperties.TestEntitlementPropertiesSerializerContext.WithOptions.TestEntitlementProperties))
            return new(await (await SendRequestAsync(HttpMethod.Post, content, $"/applications/{applicationId}/entitlements", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonEntitlement.JsonEntitlementSerializerContext.WithOptions.JsonEntitlement).ConfigureAwait(false));
    }

    public Task DeleteTestEntitlementAsync(ulong applicationId, ulong entitlementId, RequestProperties? properties = null)
        => SendRequestAsync(HttpMethod.Delete, $"/applications/{applicationId}/entitlements/{entitlementId}", null, null, properties);
}
