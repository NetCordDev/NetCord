namespace NetCord;

public abstract class EmbedPartBase : IJsonModel<JsonModels.JsonEmbedPartBase>
{
    JsonModels.JsonEmbedPartBase IJsonModel<JsonModels.JsonEmbedPartBase>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedPartBase _jsonModel;

    public string? Url => _jsonModel.Url;
    public string? ProxyUrl => _jsonModel.ProxyUrl;
    public int? Height => _jsonModel.Height;
    public int? Width => _jsonModel.Width;

    public EmbedPartBase(JsonModels.JsonEmbedPartBase jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
