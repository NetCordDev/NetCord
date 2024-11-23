using NetCord.JsonModels;

namespace NetCord;

public interface IUnknownComponent : IComponent
{
    public ComponentType Type { get; }
}

internal class UnknownComponent(JsonComponent jsonModel) : IUnknownComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
}
