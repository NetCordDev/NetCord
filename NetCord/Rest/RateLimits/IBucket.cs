using NetCord.Rest.HttpClients;

namespace NetCord.Rest.RateLimits;

internal interface IBucket
{
    public Task<HttpResponseMessage> SendAsync(IHttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties);
}
