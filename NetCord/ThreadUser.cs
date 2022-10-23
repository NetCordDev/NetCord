using NetCord.Rest;

namespace NetCord;

public class ThreadUser : ClientEntity, IJsonModel<JsonModels.JsonThreadUser>
{
    JsonModels.JsonThreadUser IJsonModel<JsonModels.JsonThreadUser>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadUser _jsonModel;

    public override ulong Id => _jsonModel.UserId;
    public ulong ThreadId => _jsonModel.ThreadId;
    public DateTimeOffset JoinTimestamp => _jsonModel.JoinTimestamp;
    public int Flags => _jsonModel.Flags;

    public ThreadUser(JsonModels.JsonThreadUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public override string ToString() => $"<@{Id}>";

    #region Channel
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildThreadUserAsync(ThreadId, Id, properties);
    #endregion
}
