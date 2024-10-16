namespace NetCord;

public abstract class InteractionData : IJsonModel<JsonModels.JsonInteractionData>
{
    private protected InteractionData(JsonModels.JsonInteractionData jsonModel)
    {
        _jsonModel = jsonModel;
    }

    JsonModels.JsonInteractionData IJsonModel<JsonModels.JsonInteractionData>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteractionData _jsonModel;
}
