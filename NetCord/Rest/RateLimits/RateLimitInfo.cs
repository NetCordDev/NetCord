namespace NetCord.Rest.RateLimits;

public class RateLimitInfo
{
    public RateLimitInfo(long timestamp, int resetAfter, int remaining, int limit, BucketInfo bucketInfo)
    {
        Timestamp = timestamp;
        ResetAfter = resetAfter;
        Remaining = remaining;
        Limit = limit;
        BucketInfo = bucketInfo;
    }

    public long Timestamp { get; }
    public long Reset => Timestamp + ResetAfter;
    public int ResetAfter { get; }
    public int Remaining { get; }
    public int Limit { get; }
    public BucketInfo BucketInfo { get; }
}
