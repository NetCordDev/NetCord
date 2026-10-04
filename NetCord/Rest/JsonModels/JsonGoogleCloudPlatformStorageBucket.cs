using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonGoogleCloudPlatformStorageBucket
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; }

    [JsonPropertyName("upload_filename")]
    public string UploadFileName { get; set; }
}
