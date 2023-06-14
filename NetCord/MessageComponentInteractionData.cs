namespace NetCord;

public abstract class MessageComponentInteractionData : InteractionData, ICustomIdInteractionData
{
    private protected MessageComponentInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }

    public string CustomId => _jsonModel.CustomId!;

    public ComponentType ComponentType => _jsonModel.ComponentType.GetValueOrDefault();
}
