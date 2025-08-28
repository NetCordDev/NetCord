using NetCord.JsonModels;

namespace NetCord;

public class ComponentSection(JsonComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public IComponentSectionAccessoryComponent Accessory { get; } = IComponentSectionAccessoryComponent.CreateFromJson(jsonModel.Accessory!);
    public IReadOnlyList<IComponentSectionComponent> Components { get; } = jsonModel.Components!.Select(c => IComponentSectionComponent.CreateFromJson(c)).ToArray();
}
