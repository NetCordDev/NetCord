namespace NetCord;

public interface IComponent
{
    public ComponentType ComponentType { get; }
    public string CustomId { get; }

    public static IComponent CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        if (jsonModel.Components[0].Type == ComponentType.StringMenu)
        {
            return new Menu(jsonModel);
        }
        else
        {
            return new ActionRow(jsonModel);
        }
    }
}
