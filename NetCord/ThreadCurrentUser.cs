namespace NetCord;

public class ThreadCurrentUser(JsonModels.JsonThreadCurrentUser jsonModel) : IJsonModel<JsonModels.JsonThreadCurrentUser>
{
    JsonModels.JsonThreadCurrentUser IJsonModel<JsonModels.JsonThreadCurrentUser>.JsonModel => jsonModel;

    public DateTimeOffset JoinTimestamp => jsonModel.JoinTimestamp;
    public int Flags => jsonModel.Flags;
}
