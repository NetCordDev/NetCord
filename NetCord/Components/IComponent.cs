namespace NetCord;

public interface IComponent
{
    public ComponentType ComponentType { get; }

    internal static IComponent CreateFromJson(JsonModels.JsonComponent jsonEntity)
    {
        if (jsonEntity.Components[0].Type == ComponentType.Menu)
        {
            return new Menu(jsonEntity);
        }
        else
        {
            return new ActionRow(jsonEntity);
        }
    }
}
