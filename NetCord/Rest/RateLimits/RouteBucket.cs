namespace NetCord.Rest.RateLimits;

internal class RouteBucket : NonGlobalRouteBucket
{
    public override async Task<HttpResponseMessage> SendAsync(HttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        var semaphore = _semaphore;
        await semaphore!.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var globalReset = _client._globalRateLimitReset;
                if (now < globalReset)
                    throw new RateLimitedException(globalReset, true);
                if (Remaining == 0)
                {
                    var reset = Reset;
                    if (now < reset)
                        throw new RateLimitedException(reset, false);
                }

                var response = await client.SendAsync(message()).ConfigureAwait(false);

                if (response.Headers.TryGetValues("x-ratelimit-remaining", out var values))
                    Remaining = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
                if (response.Headers.TryGetValues("x-ratelimit-reset", out values))
                    Reset = ParseReset(values.First());
                if (response.Headers.TryGetValues("x-ratelimit-limit", out values))
                {
                    int newLimit = int.Parse(values.First());
                    if (_limit != newLimit)
                        _semaphore = new(_limit = newLimit);
                }

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    if (response.Headers.Contains("x-ratelimit-global"))
                    {
                        var newGlobalReset = GetNewGlobalReset(response);
                        if (_client._globalRateLimitReset < newGlobalReset)
                            throw new RateLimitedException(_client._globalRateLimitReset = newGlobalReset, true);
                        else
                            throw new RateLimitedException(_client._globalRateLimitReset, true);
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
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var globalReset = _client._globalRateLimitReset;
                    if (now < globalReset)
                    {
                        await Task.Delay((int)(globalReset - now)).ConfigureAwait(false);
                        if (Remaining == 0)
                        {
                            now = globalReset;
                            var reset = Reset;
                            if (now < reset)
                                await Task.Delay((int)(reset - now)).ConfigureAwait(false);
                        }
                    }
                    else if (Remaining == 0)
                    {
                        var reset = Reset;
                        if (now < reset)
                            await Task.Delay((int)(reset - now)).ConfigureAwait(false);
                    }

                    var response = await client.SendAsync(message()).ConfigureAwait(false);

                    if (response.Headers.TryGetValues("x-ratelimit-remaining", out var values))
                        Remaining = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
                    if (response.Headers.TryGetValues("x-ratelimit-reset", out values))
                        Reset = ParseReset(values.First());
                    if (response.Headers.TryGetValues("x-ratelimit-limit", out values))
                    {
                        int newLimit = int.Parse(values.First());
                        if (_limit != newLimit)
                            _semaphore = new(_limit = newLimit);
                    }

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
            semaphore.Release();
        }
    }

    public RouteBucket(RestClient client) : base(client)
    {
        _semaphore = new(50);
    }
}
