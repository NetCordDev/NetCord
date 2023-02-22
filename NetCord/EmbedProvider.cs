namespace NetCord;

public class EmbedProvider : IJsonModel<JsonModels.JsonEmbedProvider>
{
    JsonModels.JsonEmbedProvider IJsonModel<JsonModels.JsonEmbedProvider>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedProvider _jsonModel;

    public EmbedProvider(JsonModels.JsonEmbedProvider jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? Name => _jsonModel.Name;
    public string? Url => _jsonModel.Url;
}
