namespace NetCord;

public interface IMessageComponent : IComponent
{
    public static IMessageComponent CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        var firstComponent = jsonModel.Components[0];
        return firstComponent.Type is ComponentType.Button ? new ActionRow(jsonModel) : Menu.CreateFromJson(firstComponent);
    }
}
