namespace NetCord.Rest.RateLimits;

internal class RouteRateLimiter : IRouteRateLimiter
{
    private readonly object _lock = new();

    private long _reset;
    private int _maxResetAfter;
    private int _remaining;
    private readonly int _limit;

    public BucketInfo BucketInfo { get; }

    public RouteRateLimiter(RateLimitInfo rateLimitInfo)
    {
        _reset = rateLimitInfo.Reset;
        _maxResetAfter = rateLimitInfo.ResetAfter;
        _remaining = rateLimitInfo.Remaining;
        _limit = rateLimitInfo.Limit;
        BucketInfo = rateLimitInfo.BucketInfo;
    }

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync()
    {
        var timestamp = Environment.TickCount64;
        lock (_lock)
        {
            var diff = _reset - timestamp;
            if (diff <= 0)
            {
                _remaining = _limit - 1;
                _reset = timestamp + _maxResetAfter;
            }
            else
            {
                if (_remaining == 0)
                    return new(RateLimitAcquisitionResult.RateLimit((int)diff));
                else
                    _remaining--;
            }
        }
        return new(RateLimitAcquisitionResult.NoRateLimit());
    }

    public ValueTask CancelAcquireAsync(long timestamp)
    {
        lock (_lock)
        {
            if (timestamp - (_reset - _maxResetAfter) >= -50 && _remaining < _limit)
                _remaining++;
        }
        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo)
    {
        lock (_lock)
        {
            if (rateLimitInfo.Reset - _reset >= -50)
            {
                var remaining = rateLimitInfo.Remaining;
                if (remaining < _remaining)
                    _remaining = remaining;

                _reset = rateLimitInfo.Reset;
            }

            var resetAfter = rateLimitInfo.ResetAfter;
            if (resetAfter > _maxResetAfter)
                _maxResetAfter = resetAfter;
        }
        return default;
    }

    public ValueTask IndicateExchangeAsync(long timestamp) => CancelAcquireAsync(timestamp);
}
