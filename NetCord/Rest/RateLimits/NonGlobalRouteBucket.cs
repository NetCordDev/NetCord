namespace NetCord.Rest.RateLimits;

internal class NonGlobalRouteBucket : NoRateLimitBucket
{
    private protected readonly AdjustableSemaphoreSlim _semaphore;
    private readonly object _lock = new();

    public int Remaining { get; protected set; }
    public long Reset { get; protected set; }

    public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var now = Environment.TickCount64;
                var reset = GetRouteReset();
                if (Remaining == 0 && HasRateLimit(now, reset, out _))
                    throw new RateLimitedException(reset, false);

                var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                UpdateBucket(response);

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new RateLimitedException(Reset, false);
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
                    var reset = GetRouteReset();
                    if (Remaining == 0 && HasRateLimit(now, reset, out int diff))
                        await Task.Delay(diff).ConfigureAwait(false);

                    var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                    UpdateBucket(response);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                        throw new RestException(response);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }

    public NonGlobalRouteBucket(RestClient client) : base(client)
    {
        _semaphore = new(Remaining = int.MaxValue);
    }

    protected NonGlobalRouteBucket(int startLimit, RestClient client) : base(client)
    {
        Remaining = int.MaxValue;
        _semaphore = new(startLimit);
    }

    protected long GetRouteReset()
    {
        lock (_lock)
            return Reset;
    }

    protected static bool HasRateLimit(long now, long reset, out int diff)
    {
        diff = (int)(reset - now);
        return diff > 0;
    }

    protected void UpdateBucket(HttpResponseMessage response)
    {
        var headers = response.Headers;
        lock (_lock)
        {
            var now = Environment.TickCount64;
            if (headers.TryGetValues("x-ratelimit-remaining", out var values))
            {
                var newRemaining = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
                if (headers.TryGetValues("x-ratelimit-reset-after", out values))
                {
                    var newReset = now + (long)(float.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture) * 1000);
                    long diff = newReset - Reset;
                    if (diff is >= -50 and <= 50)
                    {
                        if (Remaining > newRemaining)
                            Remaining = newRemaining;
                    }
                    else if (diff > 0)
                    {
                        Reset = newReset;
                        Remaining = newRemaining;
                    }
                }
                else if (Reset < now)
                {
                    if (Remaining > newRemaining)
                        Remaining = newRemaining;
                }
            }
            else if (headers.TryGetValues("x-ratelimit-reset-after", out values))
            {
                var newReset = now + (long)(float.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture) * 1000);
                if (newReset > Reset)
                    Reset = newReset;
            }

            if (headers.TryGetValues("x-ratelimit-limit", out values))
            {
                int newLimit = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
                if (_semaphore.MaxCount != newLimit)
                    _semaphore.MaxCount = newLimit;
            }
        }
    }
}
