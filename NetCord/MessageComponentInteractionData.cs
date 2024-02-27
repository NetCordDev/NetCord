namespace NetCord;

public abstract class MessageComponentInteractionData : ComponentInteractionData
{
    private protected MessageComponentInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }

    public ComponentType ComponentType => _jsonModel.ComponentType.GetValueOrDefault();
}
