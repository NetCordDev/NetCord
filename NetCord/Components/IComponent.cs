using NetCord.JsonModels;

namespace NetCord;

public interface IComponent
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public int Id { get; }
}

public interface IInteractiveComponent : IComponent
{
    /// <summary>
    /// Developer-defined identifier for the component (max 100 characters).
    /// </summary>
    public string CustomId { get; }
}

public interface IUnknownComponent : IComponent
{
    public ComponentType Type { get; }
}

public class ActionRowComponent(JsonActionRowComponent jsonModel) : IMessageChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public IReadOnlyList<IActionRowChildComponent> Components { get; } = [.. jsonModel.Components.Select(IActionRowChildComponent.CreateFromJson)];
}

public interface IActionRowChildComponent : IComponent
{
    public static IActionRowChildComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButtonComponent.CreateFromJson((JsonButtonComponent)jsonModel),
            _ => new UnknownActionRowChildComponent(jsonModel),
        };
    }
}

internal class UnknownActionRowChildComponent(JsonComponent jsonModel) : IUnknownComponent, IActionRowChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public interface IButtonComponent : IActionRowChildComponent, IComponentSectionAccessoryComponent
{
    public bool Disabled { get; }

    public static IButtonComponent CreateFromJson(JsonButtonComponent jsonModel)
    {
        return jsonModel.Style switch
        {
            (ButtonStyle)5 => new LinkButtonComponent(jsonModel),
            (ButtonStyle)6 => new PremiumButtonComponent(jsonModel),
            _ => new ButtonComponent(jsonModel),
        };
    }
}

public interface ICustomizableButtonComponent : IButtonComponent
{
    public string? Label { get; }

    public EmojiReference? Emoji { get; }
}

public class ButtonComponent(JsonButtonComponent jsonModel) : IInteractiveComponent, ICustomizableButtonComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId!;

    public ButtonStyle Style { get; } = jsonModel.Style;

    public string? Label { get; } = jsonModel.Label;

    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;

    public bool Disabled { get; } = jsonModel.Disabled.GetValueOrDefault();
}

public class LinkButtonComponent(JsonButtonComponent jsonModel) : ICustomizableButtonComponent
{
    public int Id { get; } = jsonModel.Id;

    public string Url { get; } = jsonModel.Url!;

    public string? Label { get; } = jsonModel.Label;

    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;

    public bool Disabled { get; } = jsonModel.Disabled.GetValueOrDefault();
}

public class PremiumButtonComponent(JsonButtonComponent jsonModel) : IButtonComponent
{
    public int Id { get; } = jsonModel.Id;
    public ulong SkuId { get; } = jsonModel.SkuId.GetValueOrDefault();
    public bool Disabled { get; } = jsonModel.Disabled.GetValueOrDefault();
}

public abstract class SelectComponent(JsonSelectComponent jsonModel, int parentId) : IInteractiveComponent, IMessageChildComponent, ILabelChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId;

    public string? Placeholder { get; } = jsonModel.Placeholder;

    public int? MinValues { get; } = jsonModel.MinValues;

    public int? MaxValues { get; } = jsonModel.MaxValues;

    public bool? Required { get; } = jsonModel.Required;

    public bool? Disabled { get; } = jsonModel.Disabled;

    public int ParentId { get; } = parentId;
}

public class StringSelectComponent(JsonStringSelectComponent jsonModel, int parentId) : SelectComponent(jsonModel, parentId)
{
    public IReadOnlyList<StringSelectOption> Options { get; } = [.. jsonModel.Options.SelectOrEmpty(o => new StringSelectOption(o))];
}

public class TextInputComponent(JsonTextInputComponent jsonModel) : IInteractiveComponent, ILabelChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId;

    public TextInputStyle Style { get; } = jsonModel.Style;

    public int? MinLength { get; } = jsonModel.MinLength;

    public int? MaxLength { get; } = jsonModel.MaxLength;

    public bool? Required { get; } = jsonModel.Required;

    public string? Value { get; } = jsonModel.Value;

    public string? Placeholder { get; } = jsonModel.Placeholder;
}

public abstract class EntitySelectComponent(JsonEntitySelectComponent jsonModel, int parentId) : SelectComponent(jsonModel, parentId)
{
}

public class UserSelectComponent(JsonUserSelectComponent jsonModel, int parentId) : EntitySelectComponent(jsonModel, parentId)
{
    public IReadOnlyList<ulong>? DefaultValues { get; } = jsonModel.DefaultValues?.Select(v => v.Id).ToArray();
}

public class RoleSelectComponent(JsonRoleSelectComponent jsonModel, int parentId) : EntitySelectComponent(jsonModel, parentId)
{
    public IReadOnlyList<ulong>? DefaultValues { get; } = jsonModel.DefaultValues?.Select(v => v.Id).ToArray();
}

public class MentionableMenuComponent(JsonMentionableSelectComponent jsonModel, int parentId) : EntitySelectComponent(jsonModel, parentId)
{
    public IReadOnlyList<MentionableMenuDefaultValue>? DefaultValues { get; } = jsonModel.DefaultValues?.Select(d => new MentionableMenuDefaultValue(d)).ToArray();
}

public class ChannelMenuComponent(JsonChannelSelectComponent jsonModel, int parentId) : EntitySelectComponent(jsonModel, parentId)
{
    public IReadOnlyList<ulong>? DefaultValues { get; } = jsonModel.DefaultValues?.Select(v => v.Id).ToArray();

    public IReadOnlyList<ChannelType>? ChannelTypes { get; } = jsonModel.ChannelTypes;
}

public class ComponentSectionComponent(JsonComponentSectionComponent jsonModel) : IMessageChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public IComponentSectionAccessoryComponent Accessory { get; } = IComponentSectionAccessoryComponent.CreateFromJson(jsonModel.Accessory);

    public IReadOnlyList<IComponentSectionChildComponent> Components { get; } = [.. jsonModel.Components.Select(IComponentSectionChildComponent.CreateFromJson)];
}

public interface IComponentSectionAccessoryComponent : IComponent
{
    public static IComponentSectionAccessoryComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.Button => IButtonComponent.CreateFromJson((JsonButtonComponent)jsonModel),
            ComponentType.Thumbnail => new ThumbnailComponent((JsonThumbnailComponent)jsonModel),
            _ => new UnknownSectionAccessoryComponent(jsonModel),
        };
    }
}

internal class UnknownSectionAccessoryComponent(JsonComponent jsonModel) : IUnknownComponent, IComponentSectionAccessoryComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public interface IComponentSectionChildComponent : IComponent
{
    public static IComponentSectionChildComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextDisplay => new TextDisplayComponent((JsonTextDisplayComponent)jsonModel),
            _ => new UnknownSectionChildComponent(jsonModel),
        };
    }
}

internal class UnknownSectionChildComponent(JsonComponent jsonModel) : IUnknownComponent, IComponentSectionChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public class TextDisplayComponent(JsonTextDisplayComponent jsonModel) : IMessageChildComponent, IModalChildComponent, IContainerChildComponent, IComponentSectionChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string Content { get; } = jsonModel.Content;
}

public class ThumbnailComponent(JsonThumbnailComponent jsonModel) : IComponentSectionAccessoryComponent
{
    public int Id { get; } = jsonModel.Id;

    public ComponentMedia Media { get; } = new(jsonModel.Media);

    public string? Description { get; } = jsonModel.Description;

    public bool Spoiler { get; } = jsonModel.Spoiler.GetValueOrDefault();
}

public class MediaGalleryComponent(JsonMediaGalleryComponent jsonModel) : IMessageChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public IReadOnlyList<MediaGalleryItem> Items { get; } = [.. jsonModel.Items.Select(c => new MediaGalleryItem(c))];
}

public class MediaGalleryItem(JsonMediaGalleryItem jsonModel)
{
    public ComponentMedia Media { get; } = new(jsonModel.Media);

    public string? Description { get; } = jsonModel.Description;

    public bool Spoiler { get; } = jsonModel.Spoiler.GetValueOrDefault();
}

public class FileComponent(JsonFileComponent jsonModel) : IMessageChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public ComponentMedia File { get; } = new(jsonModel.File);

    public bool Spoiler { get; } = jsonModel.Spoiler.GetValueOrDefault();

    public string? Name { get; } = jsonModel.Name;

    public int? Size { get; } = jsonModel.Size;
}

public class SeparatorComponent(JsonSeparatorComponent jsonModel) : IMessageChildComponent, IContainerChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public bool Divider { get; } = jsonModel.Divider.GetValueOrDefault();

    public SeparatorSpacingSize Spacing { get; } = jsonModel.Spacing.GetValueOrDefault(SeparatorSpacingSize.Small);
}

public class ContainerComponent(JsonContainerComponent jsonModel) : IMessageChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public IReadOnlyList<IContainerChildComponent> Components { get; } = [.. jsonModel.Components.Select(IContainerChildComponent.CreateFromJson)];

    public Color? AccentColor { get; } = jsonModel.AccentColor;

    public bool Spoiler { get; } = jsonModel.Spoiler.GetValueOrDefault();
}

public interface IContainerChildComponent : IComponent
{
    public static IContainerChildComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow((JsonActionRowComponent)jsonModel),
            ComponentType.TextDisplay => new TextDisplayComponent((JsonTextDisplayComponent)jsonModel),
            ComponentType.Section => new ComponentSectionComponent((JsonComponentSectionComponent)jsonModel),
            ComponentType.MediaGallery => new MediaGalleryComponent((JsonMediaGalleryComponent)jsonModel),
            ComponentType.Separator => new SeparatorComponent((JsonSeparatorComponent)jsonModel),
            ComponentType.File => new FileComponent((JsonFileComponent)jsonModel),
            _ => new UnknownContainerChildComponent(jsonModel),
        };
    }

    private static IContainerChildComponent HandleActionRow(JsonActionRowComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownContainerChildComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRowComponent(jsonModel),
            ComponentType.StringSelect => new StringSelectComponent((JsonStringSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.UserSelect => new UserSelectComponent((JsonUserSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.RoleSelect => new RoleSelectComponent((JsonRoleSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.MentionableSelect => new MentionableMenuComponent((JsonMentionableSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.ChannelSelect => new ChannelMenuComponent((JsonChannelSelectComponent)firstComponent, jsonModel.Id),
            _ => new UnknownContainerChildComponent(jsonModel),
        };
    }
}

internal class UnknownContainerChildComponent(JsonComponent jsonModel) : IUnknownComponent, IContainerChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public class LabelComponent(JsonLabelComponent jsonModel) : IModalChildComponent
{
    public int Id => jsonModel.Id;

    public string Label => jsonModel.Label;

    public string? Description => jsonModel.Description;

    public ILabelChildComponent Component { get; } = ILabelChildComponent.CreateFromJson(jsonModel.Component, jsonModel.Id);
}

public interface ILabelChildComponent : IComponent
{
    public static ILabelChildComponent CreateFromJson(JsonComponent jsonModel, int labelId)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextInput => new TextInputComponent((JsonTextInputComponent)jsonModel),
            ComponentType.StringSelect => new StringSelectComponent((JsonStringSelectComponent)jsonModel, labelId),
            ComponentType.UserSelect => new UserSelectComponent((JsonUserSelectComponent)jsonModel, labelId),
            ComponentType.RoleSelect => new RoleSelectComponent((JsonRoleSelectComponent)jsonModel, labelId),
            ComponentType.MentionableSelect => new MentionableMenuComponent((JsonMentionableSelectComponent)jsonModel, labelId),
            ComponentType.ChannelSelect => new ChannelMenuComponent((JsonChannelSelectComponent)jsonModel, labelId),
            ComponentType.FileUpload => new FileUploadComponent((JsonFileUploadComponent)jsonModel),
            ComponentType.RadioGroup => new RadioGroupComponent((JsonRadioGroupComponent)jsonModel),
            ComponentType.CheckboxGroup => new CheckboxGroupComponent((JsonCheckboxGroupComponent)jsonModel),
            ComponentType.Checkbox => new CheckboxComponent((JsonCheckboxComponent)jsonModel),
            _ => new UnknownLabelChildComponent(jsonModel),
        };
    }
}

internal class UnknownLabelChildComponent(JsonComponent jsonModel) : IUnknownComponent, ILabelChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public class FileUploadComponent(JsonFileUploadComponent jsonModel) : IInteractiveComponent, ILabelChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId;

    public int? MinValues { get; } = jsonModel.MinValues;

    public int? MaxValues { get; } = jsonModel.MaxValues;

    public bool? Required { get; } = jsonModel.Required;

    public IReadOnlyList<string>? FileTypes { get; } = jsonModel.FileTypes;
}

public class RadioGroupComponent(JsonRadioGroupComponent jsonModel) : IInteractiveComponent, ILabelChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId;

    public IReadOnlyList<RadioGroupOption> Options { get; } = [.. jsonModel.Options.Select(o => new RadioGroupOption(o))];

    public bool? Required { get; } = jsonModel.Required;
}

public class RadioGroupOption(JsonRadioGroupOption jsonModel)
{
    public string Value { get; } = jsonModel.Value;

    public string Label { get; } = jsonModel.Label;

    public string? Description { get; } = jsonModel.Description;

    public bool? Default { get; } = jsonModel.Default;
}

public class CheckboxGroupComponent(JsonCheckboxGroupComponent jsonModel) : IInteractiveComponent, ILabelChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId => jsonModel.CustomId;

    public IReadOnlyList<CheckboxGroupOption> Options { get; } = [.. jsonModel.Options.Select(o => new CheckboxGroupOption(o))];

    public int? MinValues { get; } = jsonModel.MinValues;

    public int? MaxValues { get; } = jsonModel.MaxValues;

    public bool? Required { get; } = jsonModel.Required;
}

public class CheckboxGroupOption(JsonCheckboxGroupOption jsonModel)
{
    public string Value { get; } = jsonModel.Value;

    public string Label { get; } = jsonModel.Label;

    public string? Description { get; } = jsonModel.Description;

    public bool? Default { get; } = jsonModel.Default;
}

public class CheckboxComponent(JsonCheckboxComponent jsonModel) : IInteractiveComponent, ILabelChildComponent
{
    public int Id { get; } = jsonModel.Id;

    public string CustomId { get; } = jsonModel.CustomId;

    public bool? Default { get; } = jsonModel.Default;
}

public interface IMessageChildComponent : IComponent
{
    public static IMessageChildComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.ActionRow => HandleActionRow((JsonActionRowComponent)jsonModel),
            ComponentType.Section => new ComponentSectionComponent((JsonComponentSectionComponent)jsonModel),
            ComponentType.TextDisplay => new TextDisplayComponent((JsonTextDisplayComponent)jsonModel),
            ComponentType.MediaGallery => new MediaGalleryComponent((JsonMediaGalleryComponent)jsonModel),
            ComponentType.File => new FileComponent((JsonFileComponent)jsonModel),
            ComponentType.Separator => new SeparatorComponent((JsonSeparatorComponent)jsonModel),
            ComponentType.Container => new ContainerComponent((JsonContainerComponent)jsonModel),
            _ => new UnknownMessageChildComponent(jsonModel),
        };
    }

    private static IMessageChildComponent HandleActionRow(JsonActionRowComponent jsonModel)
    {
        if (jsonModel.Components is not [var firstComponent, ..])
            return new UnknownMessageChildComponent(jsonModel);

        return firstComponent.Type switch
        {
            ComponentType.Button => new ActionRowComponent(jsonModel),
            ComponentType.StringSelect => new StringSelectComponent((JsonStringSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.UserSelect => new UserSelectComponent((JsonUserSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.RoleSelect => new RoleSelectComponent((JsonRoleSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.MentionableSelect => new MentionableMenuComponent((JsonMentionableSelectComponent)firstComponent, jsonModel.Id),
            ComponentType.ChannelSelect => new ChannelMenuComponent((JsonChannelSelectComponent)firstComponent, jsonModel.Id),
            _ => new UnknownMessageChildComponent(jsonModel),
        };
    }
}

internal class UnknownMessageChildComponent(JsonComponent jsonModel) : IUnknownComponent, IMessageChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public interface IModalChildComponent : IComponent
{
    public static IModalChildComponent CreateFromJson(JsonComponent jsonModel)
    {
        return jsonModel.Type switch
        {
            ComponentType.TextDisplay => new TextDisplayComponent((JsonTextDisplayComponent)jsonModel),
            ComponentType.Label => new LabelComponent((JsonLabelComponent)jsonModel),
            _ => new UnknownModalChildComponent(jsonModel),
        };
    }
}

internal class UnknownModalChildComponent(JsonComponent jsonModel) : IUnknownComponent, IModalChildComponent
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}
