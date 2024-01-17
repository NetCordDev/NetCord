namespace NetCord.Rest.RateLimits;

internal interface ITrackingRouteRateLimiter : IRouteRateLimiter
{
    public long LastAccess { get; }

    public void IndicateAccess();
}
