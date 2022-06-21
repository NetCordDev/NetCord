namespace NetCord.Gateway;

public class UserActivityTimestamps : IJsonModel<JsonModels.JsonUserActivityTimestamps>
{
    JsonModels.JsonUserActivityTimestamps IJsonModel<JsonModels.JsonUserActivityTimestamps>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonUserActivityTimestamps _jsonModel;

    public DateTimeOffset? StartTime => _jsonModel.StartTime;
    public DateTimeOffset? EndTime => _jsonModel.EndTime;

    public UserActivityTimestamps(JsonModels.JsonUserActivityTimestamps jsonModel)
    {
        _jsonModel = jsonModel;
    }
}