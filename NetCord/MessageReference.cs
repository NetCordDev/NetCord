using NetCord.JsonModels;

namespace NetCord;

public class MessageReference : IJsonModel<JsonMessageReference>
{
    JsonMessageReference IJsonModel<JsonMessageReference>.JsonModel => _jsonModel;
    private readonly JsonMessageReference _jsonModel;

    public Snowflake MessageId => _jsonModel.MessageId.GetValueOrDefault();
    public Snowflake ChannelId => _jsonModel.ChannelId.GetValueOrDefault();
    public Snowflake? GuildId => _jsonModel.GuildId;
    public bool? FailIfNotExists => _jsonModel.FailIfNotExists;


    public MessageReference(JsonModels.JsonMessageReference jsonModel)
    {
        _jsonModel = jsonModel;
    }
}