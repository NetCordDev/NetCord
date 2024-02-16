namespace NetCord.Gateway;

public class UserActivityAssets(JsonModels.JsonUserActivityAssets jsonModel) : IJsonModel<JsonModels.JsonUserActivityAssets>
{
    JsonModels.JsonUserActivityAssets IJsonModel<JsonModels.JsonUserActivityAssets>.JsonModel => jsonModel;

    public string? LargeImageId => jsonModel.LargeImageId;

    public string? LargeText => jsonModel.LargeText;

    public string? SmallImageId => jsonModel.SmallImageId;

    public string? SmallText => jsonModel.SmallText;
}
