using NetCord.JsonModels;

namespace NetCord;

internal class UnknownMessageComponent(JsonComponent jsonModel) : IMessageComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType Type => jsonModel.Type;
}
