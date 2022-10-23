using NetCord.JsonModels;

namespace NetCord;

public class MessageReference : IJsonModel<JsonMessageReference>
{
    JsonMessageReference IJsonModel<JsonMessageReference>.JsonModel => _jsonModel;
    private readonly JsonMessageReference _jsonModel;

    public ulong MessageId => _jsonModel.MessageId.GetValueOrDefault();
    public ulong ChannelId => _jsonModel.ChannelId.GetValueOrDefault();
    public ulong? GuildId => _jsonModel.GuildId;
    public bool? FailIfNotExists => _jsonModel.FailIfNotExists;


    public MessageReference(JsonModels.JsonMessageReference jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
