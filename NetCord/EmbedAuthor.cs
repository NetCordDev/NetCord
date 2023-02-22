namespace NetCord;

public class EmbedAuthor : IJsonModel<JsonModels.JsonEmbedAuthor>
{
    JsonModels.JsonEmbedAuthor IJsonModel<JsonModels.JsonEmbedAuthor>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedAuthor _jsonModel;

    public EmbedAuthor(JsonModels.JsonEmbedAuthor jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? Name => _jsonModel.Name;
    public string? Url => _jsonModel.Url;
    public string? IconUrl => _jsonModel.IconUrl;
    public string? ProxyIconUrl => _jsonModel.ProxyIconUrl;
}
