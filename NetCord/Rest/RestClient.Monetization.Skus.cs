namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyList<Sku>> GetSkusAsync(ulong applicationId, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/skus", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonSkuArray).ConfigureAwait(false)).Select(s => new Sku(s)).ToArray();
}
