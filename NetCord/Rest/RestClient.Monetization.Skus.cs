using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class RestClient
{
    public async Task<IReadOnlyList<Sku>> GetSkusAsync(ulong applicationId, RequestProperties? properties = null)
        => (await (await SendRequestAsync(HttpMethod.Get, $"/applications/{applicationId}/skus", null, null, properties).ConfigureAwait(false)).ToObjectAsync(JsonSku.JsonSkuArraySerializerContext.WithOptions.JsonSkuArray).ConfigureAwait(false)).Select(s => new Sku(s)).ToArray();
}
