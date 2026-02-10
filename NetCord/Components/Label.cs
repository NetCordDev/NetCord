using NetCord.JsonModels;

namespace NetCord;

public class Label(JsonLabelComponent jsonModel, InteractionResolvedData? resolvedData) : IModalComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public ILabelComponent Component { get; } = ILabelComponent.CreateFromJson(jsonModel.Component, jsonModel.Id, resolvedData);
}
