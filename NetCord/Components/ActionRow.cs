using NetCord.JsonModels;

namespace NetCord;

public class ActionRow(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public IReadOnlyList<IButton> Buttons { get; } = jsonModel.Components!.Select(IButton.CreateFromJson).ToArray();
}
