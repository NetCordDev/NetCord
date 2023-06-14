namespace NetCord;

public class StringMenuInteractionData : MessageComponentInteractionData
{
    public StringMenuInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
    }

    public IReadOnlyList<string> SelectedValues => _jsonModel.SelectedValues!;
}
