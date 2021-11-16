namespace NetCord;

public class UserActivityAssets
{
    private readonly JsonModels.JsonUserActivityAssets _jsonEntity;

    public string? LargeImageId => _jsonEntity.LargeImageId;

    public string? LargeText => _jsonEntity.LargeText;

    public string? SmallImageId => _jsonEntity.SmallImageId;

    public string? SmallText => _jsonEntity.SmallText;

    internal UserActivityAssets(JsonModels.JsonUserActivityAssets jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
