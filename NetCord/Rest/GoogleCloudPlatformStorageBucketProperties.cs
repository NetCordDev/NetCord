using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GoogleCloudPlatformStorageBucketProperties(string fileName, long fileSize)
{
    public GoogleCloudPlatformStorageBucketProperties(string fileName) : this(fileName, 1)
    {
    }

    [JsonPropertyName("filename")]
    public string FileName { get; set; } = fileName;

    [JsonPropertyName("file_size")]
    public long FileSize { get; set; } = fileSize;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("id")]
    public long? Id { get; set; }
}
