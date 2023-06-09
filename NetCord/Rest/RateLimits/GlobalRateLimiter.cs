namespace NetCord.Rest.RateLimits;

internal class GlobalRateLimiter : IRateLimiter
{
    private readonly object _lock = new();

    private int _remaining;
    private long _reset;

    public GlobalRateLimiter()
    {
        _remaining = 50;
    }

    public RateLimitInfo TryAcquire()
    {
        var timestamp = Environment.TickCount64;
        lock (_lock)
        {
            var diff = _reset - timestamp;
            if (diff <= 0)
            {
                _remaining = 49;
                _reset = timestamp + 1000;
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

    public void RateLimitReceived(long reset)
    {
        lock (_lock)
        {
            _remaining = 0;

            if (reset > _reset)
                _reset = reset;
        }
    }
}
