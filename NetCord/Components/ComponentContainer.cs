using NetCord.JsonModels;

namespace NetCord;

public class ComponentContainer(JsonComponentContainerComponent jsonModel) : IMessageComponent, IJsonModel<JsonComponentContainerComponent>
{
    JsonComponentContainerComponent IJsonModel<JsonComponentContainerComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public Color? AccentColor => jsonModel.AccentColor;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
    public IReadOnlyList<IComponentContainerComponent> Components { get; } = jsonModel.Components!.Select(IComponentContainerComponent.CreateFromJson).ToArray();
}
