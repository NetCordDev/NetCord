namespace NetCord;

public interface IComponent
{
    public static IComponent CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        var components = jsonModel.Components;

        if (components.Length is 0)
            return new UnknownComponent(jsonModel);

        var firstComponent = components[0];

        return jsonModel.Type switch
        {
            ComponentType.ActionRow => firstComponent.Type switch
            {
                ComponentType.Button => new ActionRow(firstComponent),
                ComponentType.StringMenu => new StringMenu(firstComponent),
                ComponentType.UserMenu => new UserMenu(firstComponent),
                ComponentType.ChannelMenu => new ChannelMenu(firstComponent),
                ComponentType.RoleMenu => new RoleMenu(firstComponent),
                ComponentType.MentionableMenu => new MentionableMenu(firstComponent),
                ComponentType.TextInput => new TextInput(firstComponent),
                _ => new UnknownComponent(jsonModel),
            },
            _ => new UnknownComponent(jsonModel),
        };
    }
}
