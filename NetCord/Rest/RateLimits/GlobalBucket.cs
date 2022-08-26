namespace NetCord.Rest.RateLimits;

internal class GlobalBucket : NoRateLimitBucket
{
    private readonly SemaphoreSlim _semaphore;

    public override async Task<HttpResponseMessage> SendAsync(HttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        await _semaphore!.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var globalReset = _client._globalRateLimitReset;
                if (now < globalReset)
                    throw new RateLimitedException(globalReset, true);

                var response = await client.SendAsync(message()).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && response.Headers.Contains("x-ratelimit-global"))
                {
                    var newGlobalReset = GetNewGlobalReset(response);
                    if (_client._globalRateLimitReset < newGlobalReset)
                        throw new RateLimitedException(_client._globalRateLimitReset = newGlobalReset, true);
                    else
                        throw new RateLimitedException(_client._globalRateLimitReset, true);
                }
                else
                    throw new RestException(response);
            }
            else
            {
                while (true)
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var globalReset = _client._globalRateLimitReset;
                    if (now < globalReset)
                        await Task.Delay((int)(globalReset - now)).ConfigureAwait(false);

                    var response = await client.SendAsync(message()).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (response.Headers.Contains("x-ratelimit-global"))
                        {
                            var newGlobalReset = GetNewGlobalReset(response);
                            if (_client._globalRateLimitReset < newGlobalReset)
                                _client._globalRateLimitReset = newGlobalReset;
                        }
                    }
                    else
                        throw new RestException(response);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public GlobalBucket(RestClient client) : base(client)
    {
        _semaphore = new(50);
    }
}
