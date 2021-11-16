namespace NetCord;

public class ThreadSelfUser
{
    private readonly JsonModels.JsonThreadSelfUser _jsonEntity;

    public DateTimeOffset JoinTimestamp => _jsonEntity.JoinTimestamp;
    public int Flags => _jsonEntity.Flags;

    internal ThreadSelfUser(JsonModels.JsonThreadSelfUser jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
