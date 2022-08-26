namespace NetCord.Rest.RateLimits;

internal class NonGlobalRouteBucket : NoRateLimitBucket
{
    private protected SemaphoreSlim? _semaphore;

    private protected int _limit;

    public int Remaining { get; protected set; } = int.MaxValue;

    public long Reset { get; protected set; }

    public override async Task<HttpResponseMessage> SendAsync(HttpClient client, Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        var semaphore = _semaphore;
        if (semaphore == null)
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                if (Remaining == 0)
                {
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
                    if (_semaphore == null)
                        _semaphore = new(_limit = int.Parse(values.First()));
                }

                if (response.IsSuccessStatusCode)
                    return response;
                else if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new RateLimitedException(Reset, false);
                else
                    throw new RestException(response);
            }
            else
            {
                while (true)
                {
                    if (Remaining == 0)
                    {
                        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
                        if (_semaphore == null)
                            _semaphore = new(_limit = int.Parse(values.First()));
                    }

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                        throw new RestException(response);
                }
            }
        }
        else
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
                {
                    if (Remaining == 0)
                    {
                        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
                        throw new RateLimitedException(Reset, false);
                    else
                        throw new RestException(response);
                }
                else
                {
                    while (true)
                    {
                        if (Remaining == 0)
                        {
                            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
                        else if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                            throw new RestException(response);
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    public NonGlobalRouteBucket(RestClient client) : base(client)
    {
    }
}
