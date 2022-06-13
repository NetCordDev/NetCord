namespace NetCord;

public class MessageSticker : ClientEntity, IJsonModel<JsonModels.JsonMessageSticker>
{
    JsonModels.JsonMessageSticker IJsonModel<JsonModels.JsonMessageSticker>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageSticker _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public StickerFormat Format => _jsonModel.Format;

    public MessageSticker(JsonModels.JsonMessageSticker jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }
}