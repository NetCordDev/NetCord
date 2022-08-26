namespace NetCord.Gateway;

public class MenuInteractionData : ButtonInteractionData, ICustomIdInteractionData
{
    public IReadOnlyList<string> SelectedValues => _jsonModel.SelectedValues!;

    public MenuInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }
}
