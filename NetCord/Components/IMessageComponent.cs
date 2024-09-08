namespace NetCord;

public interface IMessageComponent : IComponent
{
    public static IMessageComponent CreateFromJson(JsonModels.JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => jsonModel.Components[0].Type switch
            {
                ComponentType.Button => new ActionRow(jsonModel),
                ComponentType.StringMenu => new StringMenu(jsonModel),
                ComponentType.UserMenu => new UserMenu(jsonModel),
                ComponentType.ChannelMenu => new ChannelMenu(jsonModel),
                ComponentType.RoleMenu => new RoleMenu(jsonModel),
                ComponentType.MentionableMenu => new MentionableMenu(jsonModel),
                _ => new UnknownMessageComponent(jsonModel),
            },
            _ => new UnknownMessageComponent(jsonModel),
        };
    }
}
