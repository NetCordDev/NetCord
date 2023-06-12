namespace NetCord;

public class ButtonInteractionData : InteractionData, ICustomIdInteractionData
{
    public string CustomId => _jsonModel.CustomId!;

    public ComponentType ComponentType => _jsonModel.ComponentType.GetValueOrDefault();

    public ButtonInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }
}
