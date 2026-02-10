using NetCord.JsonModels;

namespace NetCord;

public interface IActionRowComponent : IComponent
{
    public static IActionRowComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButton.CreateFromJson((JsonButtonComponent)jsonModel),
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
            ComponentType.Button => IButton.CreateFromJson((JsonButtonComponent)jsonModel),
            ComponentType.Thumbnail => new Thumbnail((JsonThumbnailComponent)jsonModel),
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
            ComponentType.TextDisplay => new TextDisplay((JsonTextDisplayComponent)jsonModel),
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
            ComponentType.ActionRow => HandleActionRow((JsonActionRowComponent)jsonModel),
            ComponentType.TextDisplay => new TextDisplay((JsonTextDisplayComponent)jsonModel),
            ComponentType.Section => new ComponentSection((JsonComponentSectionComponent)jsonModel),
            ComponentType.MediaGallery => new MediaGallery((JsonMediaGalleryComponent)jsonModel),
            ComponentType.Separator => new ComponentSeparator((JsonComponentSeparatorComponent)jsonModel),
            ComponentType.File => new FileDisplay((JsonFileDisplayComponent)jsonModel),
            _ => new UnknownComponentContainerComponent(jsonModel),
        };
    }

    private static IComponentContainerComponent HandleActionRow(JsonActionRowComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownComponentContainerComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRow(jsonModel),
            ComponentType.StringMenu => new StringMenu((JsonStringMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.UserMenu => new UserMenu((JsonUserMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.RoleMenu => new RoleMenu((JsonRoleMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.MentionableMenu => new MentionableMenu((JsonMentionableMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.ChannelMenu => new ChannelMenu((JsonChannelMenuComponent)firstComponent, jsonModel.Id),
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
            ComponentType.ActionRow => HandleActionRow((JsonActionRowComponent)jsonModel),
            ComponentType.Section => new ComponentSection((JsonComponentSectionComponent)jsonModel),
            ComponentType.TextDisplay => new TextDisplay((JsonTextDisplayComponent)jsonModel),
            ComponentType.MediaGallery => new MediaGallery((JsonMediaGalleryComponent)jsonModel),
            ComponentType.File => new FileDisplay((JsonFileDisplayComponent)jsonModel),
            ComponentType.Separator => new ComponentSeparator((JsonComponentSeparatorComponent)jsonModel),
            ComponentType.Container => new ComponentContainer((JsonComponentContainerComponent)jsonModel),
            _ => new UnknownMessageComponent(jsonModel),
        };
    }

    private static IMessageComponent HandleActionRow(JsonActionRowComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownMessageComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRow(jsonModel),
            ComponentType.StringMenu => new StringMenu((JsonStringMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.UserMenu => new UserMenu((JsonUserMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.RoleMenu => new RoleMenu((JsonRoleMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.MentionableMenu => new MentionableMenu((JsonMentionableMenuComponent)firstComponent, jsonModel.Id),
            ComponentType.ChannelMenu => new ChannelMenu((JsonChannelMenuComponent)firstComponent, jsonModel.Id),
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
            ComponentType.TextDisplay => new TextDisplay((JsonTextDisplayComponent)jsonModel),
            ComponentType.Label => new Label((JsonLabelComponent)jsonModel, resolvedData),
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
            ComponentType.TextInput => new TextInput((JsonTextInputComponent)jsonModel),
            ComponentType.StringMenu => new StringMenu((JsonStringMenuComponent)jsonModel, labelId),
            ComponentType.UserMenu => new UserMenu((JsonUserMenuComponent)jsonModel, labelId, resolvedData),
            ComponentType.RoleMenu => new RoleMenu((JsonRoleMenuComponent)jsonModel, labelId, resolvedData),
            ComponentType.MentionableMenu => new MentionableMenu((JsonMentionableMenuComponent)jsonModel, labelId, resolvedData),
            ComponentType.ChannelMenu => new ChannelMenu((JsonChannelMenuComponent)jsonModel, labelId, resolvedData),
            ComponentType.FileUpload => new FileUpload((JsonFileUploadComponent)jsonModel, resolvedData),
            ComponentType.RadioGroup => new RadioGroup((JsonRadioGroupComponent)jsonModel),
            ComponentType.CheckboxGroup => new CheckboxGroup((JsonCheckboxGroupComponent)jsonModel),
            ComponentType.Checkbox => new Checkbox((JsonCheckboxComponent)jsonModel),
            _ => new UnknownLabelComponent(jsonModel),
        };
    }
}

public interface IInteractiveComponent : IComponent
{
    /// <summary>
    /// Developer-defined identifier for the component (max 100 characters).
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
