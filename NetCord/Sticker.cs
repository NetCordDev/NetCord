namespace NetCord;

public abstract class Sticker : Entity, IJsonModel<JsonModels.JsonSticker>
{
    JsonModels.JsonSticker IJsonModel<JsonModels.JsonSticker>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonSticker _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string Description => _jsonModel.Description;

    public string Tags => _jsonModel.Tags;

    public StickerFormat Format => _jsonModel.Format;

    private protected Sticker(JsonModels.JsonSticker jsonModel)
    {
        _jsonModel = jsonModel;
    }
}