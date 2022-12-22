using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal partial class JsonCreateGoogleCloudPlatformStorageBucketResult
{
    [JsonPropertyName("attachments")]
    public JsonGoogleCloudPlatformStorageBucket[] Buckets { get; set; }

    [JsonSerializable(typeof(JsonCreateGoogleCloudPlatformStorageBucketResult))]
    public partial class JsonCreateGoogleCloudPlatformStorageBucketResultSerializerContext : JsonSerializerContext
    {
        public static JsonCreateGoogleCloudPlatformStorageBucketResultSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
