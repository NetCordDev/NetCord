using NetCord.JsonModels;

namespace NetCord;

public interface IComponent
{
    public static IComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow(jsonModel),
            _ => new UnknownComponent(jsonModel),
        };
    }

    private static IComponent HandleActionRow(JsonComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRow(jsonModel),
            ComponentType.StringMenu => new StringMenu(firstComponent),
            ComponentType.UserMenu => new UserMenu(firstComponent),
            ComponentType.ChannelMenu => new ChannelMenu(firstComponent),
            ComponentType.RoleMenu => new RoleMenu(firstComponent),
            ComponentType.MentionableMenu => new MentionableMenu(firstComponent),
            ComponentType.TextInput => new TextInput(firstComponent),
            _ => new UnknownComponent(jsonModel),
        };
    }
}
