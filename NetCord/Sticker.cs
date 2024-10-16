namespace NetCord;

public abstract class Sticker : Entity, IJsonModel<JsonModels.JsonSticker>
{
    private protected Sticker(JsonModels.JsonSticker jsonModel)
    {
        _jsonModel = jsonModel;
        Tags = _jsonModel.Tags.Split(',');
    }

    JsonModels.JsonSticker IJsonModel<JsonModels.JsonSticker>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonSticker _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string Description => _jsonModel.Description;

    public IReadOnlyList<string> Tags { get; }

    public StickerFormat Format => _jsonModel.Format;

    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.Sticker(Id, format);
}
