using NetCord.JsonModels;

namespace NetCord;

public class ComponentContainer(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public Color? AccentColor => jsonModel.AccentColor;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
    public IReadOnlyList<IComponent> Components { get; } = jsonModel.Components!.Select(IComponent.CreateFromJson).ToArray();
}
