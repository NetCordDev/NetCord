using NetCord.JsonModels;

namespace NetCord;

public interface IComponent
{
    public int Id { get; }

    public static IComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow(jsonModel),
            ComponentType.Section => new ComponentSection(jsonModel),
            ComponentType.TextDisplay => new TextDisplay(jsonModel),
            // ComponentType.Thumbnail is handled differently
            ComponentType.MediaGallery => new MediaGallery(jsonModel),
            ComponentType.File => new FileDisplay(jsonModel),
            ComponentType.Separator => new ComponentSeparator(jsonModel),
            ComponentType.Container => new ComponentContainer(jsonModel),
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
            ComponentType.StringMenu => new StringMenu(firstComponent, jsonModel.Id),
            ComponentType.UserMenu => new UserMenu(firstComponent, jsonModel.Id),
            ComponentType.ChannelMenu => new ChannelMenu(firstComponent, jsonModel.Id),
            ComponentType.RoleMenu => new RoleMenu(firstComponent, jsonModel.Id),
            ComponentType.MentionableMenu => new MentionableMenu(firstComponent, jsonModel.Id),
            ComponentType.TextInput => new TextInput(firstComponent, jsonModel.Id),
            _ => new UnknownComponent(jsonModel),
        };
    }
}
