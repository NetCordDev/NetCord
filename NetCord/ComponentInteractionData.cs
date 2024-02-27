namespace NetCord;

public class ComponentInteractionData : InteractionData
{
    private protected ComponentInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }

    public string CustomId => _jsonModel.CustomId!;
}
