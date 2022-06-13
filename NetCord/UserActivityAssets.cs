namespace NetCord;

public class UserActivityAssets : IJsonModel<JsonModels.JsonUserActivityAssets>
{
    JsonModels.JsonUserActivityAssets IJsonModel<JsonModels.JsonUserActivityAssets>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonUserActivityAssets _jsonModel;

    public string? LargeImageId => _jsonModel.LargeImageId;

    public string? LargeText => _jsonModel.LargeText;

    public string? SmallImageId => _jsonModel.SmallImageId;

    public string? SmallText => _jsonModel.SmallText;

    public UserActivityAssets(JsonModels.JsonUserActivityAssets jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
