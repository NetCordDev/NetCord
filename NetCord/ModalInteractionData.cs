namespace NetCord;

public class ModalInteractionData(JsonModels.JsonInteractionData jsonModel) : ComponentInteractionData(jsonModel)
{
    public IReadOnlyList<IComponent> Components { get; } = jsonModel.Components!.Select(IComponent.CreateFromJson).ToArray();
}
