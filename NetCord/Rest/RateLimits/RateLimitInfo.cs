namespace NetCord.Rest.RateLimits;

public class RateLimitInfo(long timestamp, int resetAfter, int remaining, int limit, BucketInfo bucketInfo)
{
    public long Timestamp { get; } = timestamp;
    public long Reset => Timestamp + ResetAfter;
    public int ResetAfter { get; } = resetAfter;
    public int Remaining { get; } = remaining;
    public int Limit { get; } = limit;
    public BucketInfo BucketInfo { get; } = bucketInfo;
}
