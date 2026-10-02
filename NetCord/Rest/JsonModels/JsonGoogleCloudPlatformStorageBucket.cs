using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGoogleCloudPlatformStorageBucket
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("upload_url")]
    public required string UploadUrl { get; set; }

    [JsonPropertyName("upload_filename")]
    public required string UploadFileName { get; set; }
}
