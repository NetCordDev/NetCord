namespace NetCord;

public class ModalInteractionData(JsonModels.JsonInteractionData jsonModel) : ComponentInteractionData(jsonModel)
{
    public IReadOnlyList<TextInput> Components { get; } = jsonModel.Components!.Select(c => new TextInput(c)).ToArray();
}
