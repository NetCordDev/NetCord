namespace NetCord;

public class ModalSubmitInteractionData(JsonModels.JsonInteractionData jsonModel) : InteractionData(jsonModel), ICustomIdInteractionData
{
    public string CustomId => _jsonModel.CustomId!;

    public IReadOnlyList<TextInput> Components { get; } = jsonModel.Components!.Select(c => new TextInput(c)).ToArray();
}
