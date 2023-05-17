namespace NetCord.Gateway;

public class ModalSubmitInteractionData : InteractionData, ICustomIdInteractionData
{
    public ModalSubmitInteractionData(JsonModels.JsonInteractionData jsonModel) : base(jsonModel)
    {
        Components = jsonModel.Components!.Select(c => new TextInput(c)).ToArray();
    }

    public string CustomId => _jsonModel.CustomId!;

    public IReadOnlyList<TextInput> Components { get; }
}
