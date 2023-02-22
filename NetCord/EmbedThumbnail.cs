namespace NetCord;

public class EmbedThumbnail : IJsonModel<JsonModels.JsonEmbedThumbnail>
{
    JsonModels.JsonEmbedThumbnail IJsonModel<JsonModels.JsonEmbedThumbnail>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedThumbnail _jsonModel;

    public EmbedThumbnail(JsonModels.JsonEmbedThumbnail jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? Url => _jsonModel.Url;
    public string? ProxyUrl => _jsonModel.ProxyUrl;
    public int? Height => _jsonModel.Height;
    public int? Width => _jsonModel.Width;
}
