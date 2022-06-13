using System.Collections.ObjectModel;

using NetCord.JsonModels;

namespace NetCord;

public class ModalSubmitInteractionData : InteractionData, ICustomIdInteractionData
{
    public string CustomId => _jsonModel.CustomId!;

    public ReadOnlyCollection<TextInput> Components { get; }

    public ModalSubmitInteractionData(JsonInteractionData jsonModel) : base(jsonModel)
    {
        Components = new(jsonModel.Components!.Select(c => new TextInput(c)).ToArray());
    }
}