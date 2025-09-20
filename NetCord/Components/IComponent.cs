using NetCord.JsonModels;

namespace NetCord;

public interface IActionRowComponent : IComponent
{
    public static IActionRowComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButton.CreateFromJson(jsonModel),
            _ => new UnknownActionRowComponent(jsonModel),
        };
    }
}

public interface IComponentSectionAccessoryComponent : IComponent
{
    public static IComponentSectionAccessoryComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButton.CreateFromJson(jsonModel),
            ComponentType.Thumbnail => new ComponentSectionThumbnail(jsonModel),
            _ => new UnknownComponentSectionAccessoryComponent(jsonModel),
        };
    }
}

public interface IComponentSectionComponent : IComponent
{
    public static IComponentSectionComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextDisplay => new TextDisplay(jsonModel),
            _ => new UnknownComponentSectionComponent(jsonModel),
        };
    }
}

public interface IComponentContainerComponent : IComponent
{
    public static IComponentContainerComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow(jsonModel),
            ComponentType.TextDisplay => new TextDisplay(jsonModel),
            ComponentType.Section => new ComponentSection(jsonModel),
            ComponentType.MediaGallery => new MediaGallery(jsonModel),
            ComponentType.Separator => new ComponentSeparator(jsonModel),
            ComponentType.File => new FileDisplay(jsonModel),
            _ => new UnknownComponentContainerComponent(jsonModel),
        };
    }

    private static IComponentContainerComponent HandleActionRow(JsonComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownComponentContainerComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRow(jsonModel),
            ComponentType.StringMenu => new StringMenu(firstComponent, jsonModel.Id),
            ComponentType.UserMenu => new UserMenu(firstComponent, jsonModel.Id),
            ComponentType.RoleMenu => new RoleMenu(firstComponent, jsonModel.Id),
            ComponentType.MentionableMenu => new MentionableMenu(firstComponent, jsonModel.Id),
            ComponentType.ChannelMenu => new ChannelMenu(firstComponent, jsonModel.Id),
            _ => new UnknownComponentContainerComponent(jsonModel),
        };
    }
}

public interface IMessageComponent : IComponent
{
    public static IMessageComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow(jsonModel),
            ComponentType.Section => new ComponentSection(jsonModel),
            ComponentType.TextDisplay => new TextDisplay(jsonModel),
            ComponentType.MediaGallery => new MediaGallery(jsonModel),
            ComponentType.File => new FileDisplay(jsonModel),
            ComponentType.Separator => new ComponentSeparator(jsonModel),
            ComponentType.Container => new ComponentContainer(jsonModel),
            _ => new UnknownMessageComponent(jsonModel),
        };
    }

    private static IMessageComponent HandleActionRow(JsonComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownMessageComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRow(jsonModel),
            ComponentType.StringMenu => new StringMenu(firstComponent, jsonModel.Id),
            ComponentType.UserMenu => new UserMenu(firstComponent, jsonModel.Id),
            ComponentType.RoleMenu => new RoleMenu(firstComponent, jsonModel.Id),
            ComponentType.MentionableMenu => new MentionableMenu(firstComponent, jsonModel.Id),
            ComponentType.ChannelMenu => new ChannelMenu(firstComponent, jsonModel.Id),
            _ => new UnknownMessageComponent(jsonModel),
        };
    }
}

public interface IModalComponent : IComponent
{
    public static IModalComponent CreateFromJson(JsonComponent jsonModel, InteractionResolvedData? resolvedData)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextDisplay => new TextDisplay(jsonModel),
            ComponentType.Label => new Label(jsonModel, resolvedData),
            _ => new UnknownModalComponent(jsonModel),
        };
    }
}

public interface ILabelComponent : IComponent
{
    public static ILabelComponent CreateFromJson(JsonComponent jsonModel, int labelId, InteractionResolvedData? resolvedData)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextInput => new TextInput(jsonModel),
            ComponentType.StringMenu => new StringMenu(jsonModel, labelId),
            ComponentType.UserMenu => new UserMenu(jsonModel, labelId, resolvedData),
            ComponentType.RoleMenu => new RoleMenu(jsonModel, labelId, resolvedData),
            ComponentType.MentionableMenu => new MentionableMenu(jsonModel, labelId, resolvedData),
            ComponentType.ChannelMenu => new ChannelMenu(jsonModel, labelId, resolvedData),
            _ => new UnknownLabelComponent(jsonModel),
        };
    }
}

public interface IInteractiveComponent : IComponent
{
    /// <summary>
    /// Developer-defined identifier for the button (max 100 characters).
    /// </summary>
    public string CustomId { get; }
}

public interface IComponent
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public int Id { get; }
}
