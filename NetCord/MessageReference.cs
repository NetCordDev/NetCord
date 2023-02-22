namespace NetCord;

public class MessageReference : IJsonModel<JsonModels.JsonMessageReference>
{
    JsonModels.JsonMessageReference IJsonModel<JsonModels.JsonMessageReference>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageReference _jsonModel;

    public ulong MessageId => _jsonModel.MessageId.GetValueOrDefault();
    public ulong ChannelId => _jsonModel.ChannelId.GetValueOrDefault();
    public ulong? GuildId => _jsonModel.GuildId;
    public bool? FailIfNotExists => _jsonModel.FailIfNotExists;

    public MessageReference(JsonModels.JsonMessageReference jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
