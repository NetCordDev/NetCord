using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GoogleCloudPlatformStorageBucketProperties
{
    public GoogleCloudPlatformStorageBucketProperties(string fileName) : this(fileName, 1)
    {
    }

    public GoogleCloudPlatformStorageBucketProperties(string fileName, long fileSize)
    {
        FileName = fileName;
        FileSize = fileSize;
    }

    [JsonPropertyName("filename")]
    public string FileName { get; set; }

    [JsonPropertyName("file_size")]
    public long FileSize { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonSerializable(typeof(GoogleCloudPlatformStorageBucketProperties))]
    public partial class GoogleCloudPlatformStorageBucketPropertiesSerializerContext : JsonSerializerContext
    {
        public static GoogleCloudPlatformStorageBucketPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<GoogleCloudPlatformStorageBucketProperties>))]
    public partial class IEnumerableOfGoogleCloudPlatformStorageBucketPropertiesSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfGoogleCloudPlatformStorageBucketPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
