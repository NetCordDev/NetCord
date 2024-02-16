namespace NetCord.Gateway;

public class UserActivityTimestamps(JsonModels.JsonUserActivityTimestamps jsonModel) : IJsonModel<JsonModels.JsonUserActivityTimestamps>
{
    JsonModels.JsonUserActivityTimestamps IJsonModel<JsonModels.JsonUserActivityTimestamps>.JsonModel => jsonModel;

    public DateTimeOffset? StartTime => jsonModel.StartTime;
    public DateTimeOffset? EndTime => jsonModel.EndTime;
}
