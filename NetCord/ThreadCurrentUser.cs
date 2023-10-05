namespace NetCord;

public class ThreadCurrentUser : IJsonModel<JsonModels.JsonThreadCurrentUser>
{
    JsonModels.JsonThreadCurrentUser IJsonModel<JsonModels.JsonThreadCurrentUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadCurrentUser _jsonModel;

    public DateTimeOffset JoinTimestamp => _jsonModel.JoinTimestamp;
    public int Flags => _jsonModel.Flags;

    public ThreadCurrentUser(JsonModels.JsonThreadCurrentUser jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
