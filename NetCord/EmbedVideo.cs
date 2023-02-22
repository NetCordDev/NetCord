namespace NetCord;

public class EmbedVideo : IJsonModel<JsonModels.JsonEmbedVideo>
{
    JsonModels.JsonEmbedVideo IJsonModel<JsonModels.JsonEmbedVideo>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedVideo _jsonModel;

    public EmbedVideo(JsonModels.JsonEmbedVideo jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? Url => _jsonModel.Url;
    public string? ProxyUrl => _jsonModel.ProxyUrl;
    public int? Height => _jsonModel.Height;
    public int? Width => _jsonModel.Width;
}
