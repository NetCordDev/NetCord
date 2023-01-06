namespace NetCord.Rest.RateLimits;

internal class RouteBucket : NonGlobalRouteBucket
{
    public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var now = Environment.TickCount64;
                var reset = GetGlobalReset();
                if (HasRateLimit(now, reset, out _))
                    throw new RateLimitedException(reset, true);
                reset = GetRouteReset();
                if (Remaining == 0 && HasRateLimit(now, reset, out _))
                    throw new RateLimitedException(reset, false);

                var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                UpdateBucket(response);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (response.Headers.Contains("x-ratelimit-global"))
                    {
                        var newReset = await GetNewGlobalResetAsync(response).ConfigureAwait(false);
                        lock (_client._globalRateLimitLock)
                        {
                            reset = _client._globalRateLimitReset;
                            if (newReset > reset)
                                reset = _client._globalRateLimitReset = newReset;
                        }
                        throw new RateLimitedException(reset, true);
                    }
                    else
                        throw new RateLimitedException(Reset, false);
                }
                else
                    throw new RestException(response);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        else
        {
            while (true)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    var now = Environment.TickCount64;
                    var reset = GetGlobalReset();
                    if (HasRateLimit(now, reset, out long diff))
                    {
                        await WaitForRateLimitAsync(diff).ConfigureAwait(false);
                        now = reset;
                    }

                    reset = GetRouteReset();
                    if (Remaining == 0 && HasRateLimit(now, reset, out diff))
                        await WaitForRateLimitAsync(diff).ConfigureAwait(false);

                    var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                    UpdateBucket(response);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (response.Headers.Contains("x-ratelimit-global"))
                        {
                            var newReset = await GetNewGlobalResetAsync(response).ConfigureAwait(false);
                            lock (_client._globalRateLimitLock)
                            {
                                reset = _client._globalRateLimitReset;
                                if (newReset > reset)
                                    _client._globalRateLimitReset = newReset;
                            }
                        }
                    }
                    else
                        throw new RestException(response);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }

    public RouteBucket(RestClient client) : base(50, client)
    {
    }

    private long GetGlobalReset()
    {
        lock (_client._globalRateLimitLock)
            return _client._globalRateLimitReset;
    }
}
