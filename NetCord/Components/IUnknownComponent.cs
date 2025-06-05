using NetCord.JsonModels;

namespace NetCord;

public interface IUnknownComponent : IComponent, IComponentSectionAccessory
{
    public ComponentType Type { get; }
}

internal class UnknownComponent(JsonComponent jsonModel) : IUnknownComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentType Type => jsonModel.Type;
}
