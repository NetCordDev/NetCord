namespace NetCord.Rest.RateLimits;

internal class RouteRateLimiter(RateLimitInfo rateLimitInfo) : ITrackingRouteRateLimiter
{
    private readonly object _lock = new();

    private long _reset = rateLimitInfo.Reset;
    private int _maxResetAfter = rateLimitInfo.ResetAfter;
    private int _remaining = rateLimitInfo.Remaining;
    private readonly int _limit = rateLimitInfo.Limit;

    public bool HasBucketInfo => true;

    public BucketInfo BucketInfo { get; } = rateLimitInfo.BucketInfo;

    public long LastAccess { get; private set; } = Environment.TickCount64;

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync(CancellationToken cancellationToken = default)
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
        return new(RateLimitAcquisitionResult.NoRateLimit);
    }

    public ValueTask CancelAcquireAsync(long acquisitionTimestamp, CancellationToken cancellationToken = default)
    {
        var currentTimestamp = Environment.TickCount64;
        lock (_lock)
        {
            var reset = _reset;
            var safeStart = reset - _maxResetAfter - 50;
            if (acquisitionTimestamp <= reset
                && acquisitionTimestamp >= safeStart
                && currentTimestamp <= reset
                && currentTimestamp >= safeStart
                && _remaining < _limit)
                _remaining++;
        }
        return default;
    }

    public ValueTask UpdateAsync(RateLimitInfo rateLimitInfo, CancellationToken cancellationToken = default)
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

    public ValueTask IndicateExchangeAsync(long timestamp, CancellationToken cancellationToken = default) => CancelAcquireAsync(timestamp, cancellationToken);

    public void IndicateAccess()
    {
        LastAccess = Environment.TickCount64;
    }
}
