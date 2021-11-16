namespace NetCord;

public class UserActivityTimestamps
{
    private readonly JsonModels.JsonUserActivityTimestamps _jsonEntity;

    public DateTimeOffset? StartTime => _jsonEntity.StartTime;
    public DateTimeOffset? EndTime => _jsonEntity.EndTime;

    internal UserActivityTimestamps(JsonModels.JsonUserActivityTimestamps jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
