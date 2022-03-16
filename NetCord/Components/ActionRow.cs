using NetCord.JsonModels;

namespace NetCord;

public class ActionRow : IComponent
{
    private readonly JsonComponent _jsonEntity;

    public ComponentType ComponentType => ComponentType.ActionRow;
    public string CustomId => _jsonEntity.CustomId!;
    public IEnumerable<Button> Buttons { get; }

    internal ActionRow(JsonComponent jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Buttons = jsonEntity.Components.SelectOrEmpty(b => Button.CreateFromJson(b));
    }
}