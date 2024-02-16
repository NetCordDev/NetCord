namespace NetCord.Rest;

public class GoogleCloudPlatformStorageBucket(JsonModels.JsonGoogleCloudPlatformStorageBucket jsonModel) : IJsonModel<JsonModels.JsonGoogleCloudPlatformStorageBucket>
{
    JsonModels.JsonGoogleCloudPlatformStorageBucket IJsonModel<JsonModels.JsonGoogleCloudPlatformStorageBucket>.JsonModel => jsonModel;

    public long? Id => jsonModel.Id;
    public string UploadUrl => jsonModel.UploadUrl;
    public string UploadFileName => jsonModel.UploadFileName;
}
