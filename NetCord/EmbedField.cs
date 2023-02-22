namespace NetCord;

public class EmbedField : IJsonModel<JsonModels.JsonEmbedField>
{
    JsonModels.JsonEmbedField IJsonModel<JsonModels.JsonEmbedField>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedField _jsonModel;

    public EmbedField(JsonModels.JsonEmbedField jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string Title => _jsonModel.Title;
    public string Description => _jsonModel.Description;
    public bool? Inline => _jsonModel.Inline;
}
