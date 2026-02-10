using System.Runtime.CompilerServices;

using NetCord.JsonModels;

namespace NetCord;

public abstract class Menu(JsonMenuComponent jsonModel, int parentId) : IInteractiveComponent, IMessageComponent, ILabelComponent, IComponentContainerComponent, IJsonModel<JsonMenuComponent>
{
    JsonMenuComponent IJsonModel<JsonMenuComponent>.JsonModel => jsonModel;

    private protected T GetJsonModel<T>() where T : JsonMenuComponent => Unsafe.As<T>(jsonModel);

    public int Id => jsonModel.Id;
    public string CustomId => jsonModel.CustomId;
    public string? Placeholder => jsonModel.Placeholder;
    public int? MinValues => jsonModel.MinValues;
    public int? MaxValues => jsonModel.MaxValues;
    public bool Disabled => jsonModel.Disabled.GetValueOrDefault();
    public bool? Required => jsonModel.Required;
    public int ParentId => parentId;
}
