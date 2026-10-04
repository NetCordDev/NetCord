using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonCreateGoogleCloudPlatformStorageBucketResult
{
    [JsonPropertyName("attachments")]
    public JsonGoogleCloudPlatformStorageBucket[] Buckets { get; set; }
}
