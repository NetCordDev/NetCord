namespace NetCord.Rest.RateLimits;

internal class GlobalRateLimiter(int limit, long duration) : IGlobalRateLimiter
{
    private readonly object _lock = new();
    private readonly int _maxRemaining = limit - 1;
    private int _remaining = limit;
    private long _reset;

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default)
    {
        var timestamp = Environment.TickCount64;
        lock (_lock)
        {
            var diff = _reset - timestamp;
            if (diff <= 0)
            {
                _remaining = _maxRemaining;
                _reset = timestamp + duration;
            }
            else
            {
                if (_remaining == 0)
                    return new(RateLimitAcquisitionResult.RateLimit((int)diff));
                else
                    _remaining--;
            }
        }

        return new(RateLimitAcquisitionResult.NoRateLimit);
    }

    public ValueTask IndicateRateLimitAsync(long reset, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _remaining = 0;

            if (reset > _reset)
                _reset = reset;
        }

        return default;
    }
}
