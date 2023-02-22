namespace NetCord;

public class EmbedImage : IJsonModel<JsonModels.JsonEmbedImage>
{
    JsonModels.JsonEmbedImage IJsonModel<JsonModels.JsonEmbedImage>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedImage _jsonModel;

    public EmbedImage(JsonModels.JsonEmbedImage jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? Url => _jsonModel.Url;
    public string? ProxyUrl => _jsonModel.ProxyUrl;
    public int? Height => _jsonModel.Height;
    public int? Width => _jsonModel.Width;
}
