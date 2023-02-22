namespace NetCord;

public class EmbedFooter : IJsonModel<JsonModels.JsonEmbedFooter>
{
    JsonModels.JsonEmbedFooter IJsonModel<JsonModels.JsonEmbedFooter>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedFooter _jsonModel;

    public EmbedFooter(JsonModels.JsonEmbedFooter jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string Text => _jsonModel.Text;
    public string? IconUrl => _jsonModel.IconUrl;
    public string? ProxyIconUrl => _jsonModel.ProxyIconUrl;
}
