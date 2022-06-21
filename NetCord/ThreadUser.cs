using NetCord.Rest;

namespace NetCord;

public class ThreadUser : ClientEntity, IJsonModel<JsonModels.JsonThreadUser>
{
    JsonModels.JsonThreadUser IJsonModel<JsonModels.JsonThreadUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadUser _jsonModel;

    public override Snowflake Id => _jsonModel.UserId;
    public Snowflake ThreadId => _jsonModel.ThreadId;
    public DateTimeOffset JoinTimestamp => _jsonModel.JoinTimestamp;
    public int Flags => _jsonModel.Flags;

    public ThreadUser(JsonModels.JsonThreadUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => $"<@{Id}>";
}