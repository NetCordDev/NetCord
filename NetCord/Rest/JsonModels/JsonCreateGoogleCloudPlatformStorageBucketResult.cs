using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonCreateGoogleCloudPlatformStorageBucketResult
{
    [JsonPropertyName("attachments")]
    public JsonGoogleCloudPlatformStorageBucket[] Buckets { get; set; }
}
