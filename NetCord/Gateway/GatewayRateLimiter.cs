namespace NetCord.Gateway;

public sealed class GatewayRateLimiter(int limit, long duration) : IRateLimiter
{
    private readonly object _lock = new();
    private readonly int _limit = limit;
    private int _remaining = limit;
    private long _reset;

    public ValueTask<RateLimitAcquisitionResult> TryAcquireAsync()
    {
        var timestamp = Environment.TickCount64;
        lock (_lock)
        {
            var diff = _reset - timestamp;
            if (diff <= 0)
            {
                _remaining = _limit - 1;
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

        return new(RateLimitAcquisitionResult.NoRateLimit());
    }

    public void Reset()
    {
        lock (_lock)
        {
            _remaining = _limit;
            _reset = 0;
        }
    }

    public void Dispose()
    {
    }
}
