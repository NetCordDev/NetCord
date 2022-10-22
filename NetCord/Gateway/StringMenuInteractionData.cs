namespace NetCord.Gateway;

public class StringMenuInteractionData : ButtonInteractionData, ICustomIdInteractionData
{
    public IReadOnlyList<string> SelectedValues => _jsonModel.SelectedValues!;

    public StringMenuInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }
}
