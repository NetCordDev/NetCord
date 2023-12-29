namespace NetCord.Rest.RateLimits;

public record BucketInfo
{
    public BucketInfo(string bucket, TopLevelResourceInfo? resourceInfo)
    {
        Bucket = bucket;
        ResourceInfo = resourceInfo;
    }

    public string Bucket { get; }

    public TopLevelResourceInfo? ResourceInfo { get; }
}
