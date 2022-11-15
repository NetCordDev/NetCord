namespace NetCord.Rest.RateLimits;

internal interface IBucket
{
    public Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties);
}
