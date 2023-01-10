using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class GoogleCloudPlatformStorageBucketsProperties
{
    public GoogleCloudPlatformStorageBucketsProperties(IEnumerable<GoogleCloudPlatformStorageBucketProperties> buckets)
    {
        Buckets = buckets;
    }

    [JsonPropertyName("files")]
    public IEnumerable<GoogleCloudPlatformStorageBucketProperties> Buckets { get; set; }

    [JsonSerializable(typeof(GoogleCloudPlatformStorageBucketsProperties))]
    public partial class GoogleCloudPlatformStorageBucketsPropertiesSerializerContext : JsonSerializerContext
    {
        public static GoogleCloudPlatformStorageBucketsPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
