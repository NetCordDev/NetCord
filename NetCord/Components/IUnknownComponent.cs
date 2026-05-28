using NetCord.JsonModels;

namespace NetCord;

public interface IUnknownComponent : IComponent
{
    public ComponentType Type { get; }
}

internal class UnknownMessageComponent(JsonComponent jsonModel) : IUnknownComponent, IMessageComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownModalComponent(JsonComponent jsonModel) : IUnknownComponent, IModalComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownLabelComponent(JsonComponent jsonModel) : IUnknownComponent, ILabelComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownComponentContainerComponent(JsonComponent jsonModel) : IUnknownComponent, IComponentContainerComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownComponentSectionComponent(JsonComponent jsonModel) : IUnknownComponent, IComponentSectionComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownComponentSectionAccessoryComponent(JsonComponent jsonModel) : IUnknownComponent, IComponentSectionAccessoryComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}

internal class UnknownActionRowComponent(JsonComponent jsonModel) : IUnknownComponent, IActionRowComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
    public int Id => jsonModel.Id;
}
