using NetCord.JsonModels;

namespace NetCord.Gateway;

public class ModalSubmitInteractionData : InteractionData, ICustomIdInteractionData
{
    public string CustomId => _jsonModel.CustomId!;

    public IReadOnlyList<TextInput> Components { get; }

    public ModalSubmitInteractionData(JsonInteractionData jsonModel) : base(jsonModel)
    {
        Components = jsonModel.Components!.Select(c => new TextInput(c)).ToArray();
    }
}