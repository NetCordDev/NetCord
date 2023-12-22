using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GoogleCloudPlatformStorageBucketsProperties
{
    public GoogleCloudPlatformStorageBucketsProperties(IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets)
    {
        Buckets = buckets;
    }

    [JsonPropertyName("files")]
    public IEnumerable<GoogleCloudPlatformStorageBucketProperties> Buckets { get; set; }
}
