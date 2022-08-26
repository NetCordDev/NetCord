namespace NetCord.Rest.RateLimits;

internal interface IBucket
{
    public Task<HttpResponseMessage> SendAsync(HttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties);
}
