using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GoogleCloudPlatformStorageBucket(JsonGoogleCloudPlatformStorageBucket jsonModel)
{
    public long? Id { get; } = jsonModel.Id;

    public string UploadUrl { get; } = jsonModel.UploadUrl;

    public string UploadFileName { get; } = jsonModel.UploadFileName;
}
