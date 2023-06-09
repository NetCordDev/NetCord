namespace NetCord.Rest.RateLimits;

internal class RouteRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly int _limit;

    private int _remaining;
    private long _reset;
    private int _maxResetAfter;

    public BucketInfo BucketInfo { get; }

    public RouteRateLimiter(int limit, int remaining, long reset, int resetAfter, BucketInfo bucketInfo)
    {
        _limit = limit;
        _remaining = remaining;
        _reset = reset;
        _maxResetAfter = resetAfter;
        BucketInfo = bucketInfo;
    }

    public RateLimitInfo TryAcquire()
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
                    return new((int)diff, true);
                else
                    _remaining--;
            }
        }
        return new(0, false);
    }

    public void CancelAcquire(long timestamp)
    {
        lock (_lock)
        {
            if (timestamp - (_reset - _maxResetAfter) >= -50 && _remaining < _limit)
                _remaining++;
        }
    }

    public void Update(int newRemaining, long newReset, int newResetAfter)
    {
        lock (_lock)
        {
            if (newReset - _reset >= -50)
            {
                if (newRemaining < _remaining)
                    _remaining = newRemaining;

                _reset = newReset;
            }

            if (newResetAfter > _maxResetAfter)
                _maxResetAfter = newResetAfter;
        }
    }
}
