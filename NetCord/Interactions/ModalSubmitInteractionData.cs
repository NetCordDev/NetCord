using System.Collections.ObjectModel;

using NetCord.JsonModels;

namespace NetCord;

public class ModalSubmitInteractionData : InteractionData, ICustomIdInteractionData
{
    public string CustomId => _jsonEntity.CustomId!;

    public ReadOnlyCollection<TextInput> Components { get; }

    internal ModalSubmitInteractionData(JsonInteractionData jsonEntity) : base(jsonEntity)
    {
        Components = new(jsonEntity.Components!.Select(c => new TextInput(c)).ToArray());
    }
}