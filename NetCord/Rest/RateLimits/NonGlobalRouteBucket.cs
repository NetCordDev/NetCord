using System.Net.Http.Headers;

namespace NetCord.Rest.RateLimits;

internal class NonGlobalRouteBucket : NoRateLimitBucket
{
    private protected readonly object _semaphoreLock = new();

    private protected readonly AdjustableSemaphoreSlim _semaphore;

    private protected int _limit;

    public int Remaining { get; protected set; }

    public int Reset { get; protected set; }

    public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> message, RequestProperties? properties)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (properties != null && properties.RateLimitHandling == RateLimitHandling.NoRetry)
            {
                int now = Environment.TickCount;
                EnsureNoRateLimit(now);

                var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                UpdateData(response.Headers);

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
                    int now = Environment.TickCount;
                    if (HasRateLimit(now, out int diff))
                        await WaitForRateLimitEndAsync(diff).ConfigureAwait(false);

                    var response = await _client._httpClient.SendAsync(message()).ConfigureAwait(false);

                    UpdateData(response.Headers);

                    if (response.IsSuccessStatusCode)
                        return response;
                    else if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                        throw new RestException(response);
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public NonGlobalRouteBucket(RestClient client) : base(client)
    {
        _semaphore = new(_limit = Remaining = int.MaxValue);
    }

    protected NonGlobalRouteBucket(int startLimit, RestClient client) : base(client)
    {
        Remaining = int.MaxValue;
        _semaphore = new(_limit = startLimit);
    }

    internal void EnsureNoRateLimit(int now)
    {
        if (Remaining == 0)
        {
            var reset = Reset;
            var diff = reset - now;
            if (diff > 0)
                throw new RateLimitedException(reset, false);
        }
    }

    internal bool HasRateLimit(int now, out int diff)
    {
        if (Remaining == 0)
        {
            var reset = Reset;
            diff = reset - now;
            return diff > 0;
        }
        else
        {
            diff = 0;
            return false;
        }
    }

    internal static Task WaitForRateLimitEndAsync(int diff) => Task.Delay(diff);

    internal void UpdateData(HttpResponseHeaders headers)
    {
        if (headers.TryGetValues("x-ratelimit-remaining", out var values))
            Remaining = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
        if (headers.TryGetValues("x-ratelimit-reset-after", out values))
            Reset = GetReset(values.First());
        if (headers.TryGetValues("x-ratelimit-limit", out values))
        {
            int newLimit = int.Parse(values.First(), System.Globalization.CultureInfo.InvariantCulture);
            lock (_semaphoreLock)
            {
                int diff = _limit - newLimit;
                switch (diff)
                {
                    case 0:
                        break;
                    case > 0:
                        _limit = newLimit;
                        _semaphore.Enter(diff);
                        break;
                    default:
                        _limit = newLimit;
                        _semaphore.Release(-diff);
                        break;
                }
            }
        }
    }
}
