namespace NetCord;

public class StringMenuInteractionData(JsonModels.JsonInteractionData jsonModel) : MessageComponentInteractionData(jsonModel)
{
    public IReadOnlyList<string> SelectedValues => _jsonModel.SelectedValues!;
}
