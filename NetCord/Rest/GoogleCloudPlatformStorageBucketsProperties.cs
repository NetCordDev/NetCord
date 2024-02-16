using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GoogleCloudPlatformStorageBucketsProperties(IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets)
{
    [JsonPropertyName("files")]
    public IEnumerable<GoogleCloudPlatformStorageBucketProperties> Buckets { get; set; } = buckets;
}
