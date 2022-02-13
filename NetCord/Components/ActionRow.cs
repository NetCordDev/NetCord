using NetCord.JsonModels;

namespace NetCord;

public class ActionRow : IComponent
{
    public ComponentType ComponentType => ComponentType.ActionRow;
    public IEnumerable<Button> Buttons { get; }

    internal ActionRow(JsonComponent jsonEntity)
    {
        Buttons = jsonEntity.Components.SelectOrEmpty(b => Button.CreateFromJson(b));
    }
}