namespace NetCord.Rest.RateLimits;

internal class RouteBucket : NonGlobalRouteBucket
{
    public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                int now = Environment.TickCount;
                GlobalBucket.EnsureNoGlobalRateLimit(now, _client);
                EnsureNoRateLimit(now);

                var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                UpdateData(response.Headers);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (GlobalBucket.HasGlobalRateLimit(response.Headers))
                    {
                        await GlobalBucket.UpdateGlobalRateLimitDataAndThrowAsync(response, _client).ConfigureAwait(false);
                        throw null;
                    }
                    else
                        throw new RateLimitedException(Reset, false);
                }
                else
                    throw new RestException(response);
            }
            else
            {
                while (true)
                {
                    int now = Environment.TickCount;
                    int globalReset = _client._globalRateLimitReset;
                    if (GlobalBucket.HasGlobalRateLimit(now, globalReset, out int diff))
                    {
                        await GlobalBucket.WaitForGlobalRateLimitEndAsync(diff).ConfigureAwait(false);
                        if (HasRateLimit(globalReset, out diff))
                            await WaitForRateLimitEndAsync(diff).ConfigureAwait(false);
                    }
                    else
                    {
                        if (HasRateLimit(now, out diff))
                            await WaitForRateLimitEndAsync(diff).ConfigureAwait(false);
                    }

                    var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                    UpdateData(response.Headers);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (GlobalBucket.HasGlobalRateLimit(response.Headers))
                            await GlobalBucket.UpdateGlobalRateLimitDataAsync(response, _client).ConfigureAwait(false);
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

    public RouteBucket(RestClient client) : base(50, client)
    {
    }
}
