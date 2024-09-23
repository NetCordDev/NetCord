namespace NetCord;

public interface IMessageComponent : IComponent
{
    public static IMessageComponent CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        var component = jsonModel.Components[0];
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => component.Type switch
            {
                ComponentType.Button => new ActionRow(component),
                ComponentType.StringMenu => new StringMenu(component),
                ComponentType.UserMenu => new UserMenu(component),
                ComponentType.ChannelMenu => new ChannelMenu(component),
                ComponentType.RoleMenu => new RoleMenu(component),
                ComponentType.MentionableMenu => new MentionableMenu(component),
                _ => new UnknownMessageComponent(component),
            },
            _ => new UnknownMessageComponent(component),
        };
    }
}
