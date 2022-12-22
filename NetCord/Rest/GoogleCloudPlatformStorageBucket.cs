namespace NetCord.Rest;

public class GoogleCloudPlatformStorageBucket : IJsonModel<JsonModels.JsonGoogleCloudPlatformStorageBucket>
{
    JsonModels.JsonGoogleCloudPlatformStorageBucket IJsonModel<JsonModels.JsonGoogleCloudPlatformStorageBucket>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGoogleCloudPlatformStorageBucket _jsonModel;

    public GoogleCloudPlatformStorageBucket(JsonModels.JsonGoogleCloudPlatformStorageBucket jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public long? Id => _jsonModel.Id;
    public string UploadUrl => _jsonModel.UploadUrl;
    public string UploadFileName => _jsonModel.UploadFileName;
}
