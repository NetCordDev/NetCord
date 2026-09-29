using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonCreateGoogleCloudPlatformStorageBucketResult
{
    [JsonPropertyName("attachments")]
    public required JsonGoogleCloudPlatformStorageBucket[] Buckets { get; set; }
}
