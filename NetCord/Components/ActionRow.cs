using NetCord.JsonModels;

namespace NetCord;

public class ActionRow(JsonComponent jsonModel) : IMessageComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public IReadOnlyList<IButton> Buttons { get; } = jsonModel.Components.SelectOrEmpty(IButton.CreateFromJson).ToArray();
}
