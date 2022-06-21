namespace NetCord.Gateway;

public abstract class InteractionData : IJsonModel<JsonModels.JsonInteractionData>
{
    JsonModels.JsonInteractionData IJsonModel<JsonModels.JsonInteractionData>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteractionData _jsonModel;

    private protected InteractionData(JsonModels.JsonInteractionData jsonModel)
    {
        _jsonModel = jsonModel;
    }
}