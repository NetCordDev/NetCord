namespace NetCord.Gateway;

public class StringMenuInteractionData : ButtonInteractionData
{
    public IReadOnlyList<string> SelectedValues => _jsonModel.SelectedValues!;

    public StringMenuInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }
}
