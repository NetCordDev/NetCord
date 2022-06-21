namespace NetCord;

public class ThreadSelfUser : IJsonModel<JsonModels.JsonThreadSelfUser>
{
    JsonModels.JsonThreadSelfUser IJsonModel<JsonModels.JsonThreadSelfUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadSelfUser _jsonModel;

    public DateTimeOffset JoinTimestamp => _jsonModel.JoinTimestamp;
    public int Flags => _jsonModel.Flags;

    public ThreadSelfUser(JsonModels.JsonThreadSelfUser jsonModel)
    {
        _jsonModel = jsonModel;
    }
}