namespace NetCord;

public abstract class InteractionData
{
    private protected readonly JsonModels.JsonInteractionData _jsonEntity;

    private protected InteractionData(JsonModels.JsonInteractionData jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}