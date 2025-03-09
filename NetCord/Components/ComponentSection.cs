using NetCord.JsonModels;

namespace NetCord;

public class ComponentSection(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public IComponentSectionAccessory Accessory { get; } = IComponentSectionAccessory.CreateFromJson(jsonModel.Accessory!);
    public IReadOnlyList<TextDisplay> Components { get; } = jsonModel.Components!.Select(c => new TextDisplay(c)).ToArray();
}
