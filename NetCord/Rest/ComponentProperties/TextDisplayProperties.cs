using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TextDisplayProperties(string content) : ComponentProperties
{
    [JsonPropertyName("type")]
    public override ComponentType ComponentType => ComponentType.TextDisplay;

    [JsonPropertyName("content")]
    public string Content { get; set; } = content;
}

public partial class ThumbnailProperties(ComponentMediaProperties media) : ComponentProperties, ISectionAccessoryProperties
{
    public override ComponentType ComponentType => ComponentType.Thumbnail;

    [JsonPropertyName("media")]
    public ComponentMediaProperties Media { get; set; } = media;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }
}

[CollectionBuilder(typeof(MediaGalleryProperties), nameof(Create))]
public partial class MediaGalleryProperties(IEnumerable<MediaGalleryItemProperties> items) : ComponentProperties, IMediaGalleryProperties, IEnumerable<MediaGalleryItemProperties>
{
    public MediaGalleryProperties() : this([])
    {
    }

    public override ComponentType ComponentType => ComponentType.MediaGallery;

    [JsonPropertyName("items")]
    public IEnumerable<MediaGalleryItemProperties> Items { get; set; } = items;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(MediaGalleryItemProperties item) => AddItems(item);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<MediaGalleryItemProperties> IEnumerable<MediaGalleryItemProperties>.GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();
}

internal interface IMediaGalleryProperties
{
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }

    [JsonPropertyName("items")]
    public IEnumerable<MediaGalleryItemProperties> Items { get; }
}

public partial class MediaGalleryItemProperties(ComponentMediaProperties media)
{
    [JsonPropertyName("media")]
    public ComponentMediaProperties Media { get; set; } = media;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }
}

public partial class FileDisplayProperties(ComponentMediaProperties file) : ComponentProperties
{
    public override ComponentType ComponentType => ComponentType.File;

    [JsonPropertyName("file")]
    public ComponentMediaProperties File { get; set; } = file;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }
}

public partial class SeparatorProperties : ComponentProperties
{
    public override ComponentType ComponentType => ComponentType.Separator;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spacing")]
    public SeparatorSpacingSize? Spacing { get; set; }
}

public enum SeparatorSpacingSize
{
    Small = 1,
    Large = 2,
}

public partial class ContainerProperties(IEnumerable<ComponentProperties> components) : ComponentProperties, IContainerProperties, IEnumerable<ComponentProperties>
{
    public ContainerProperties() : this([])
    {
    }

    public override ComponentType ComponentType => ComponentType.Container;

    public Color? AccentColor { get; set; }

    public bool Spoiler { get; set; }

    public IEnumerable<ComponentProperties> Components { get; set; } = components;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(ComponentProperties component) => AddComponents(component);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<ComponentProperties> IEnumerable<ComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

internal interface IContainerProperties
{
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; }

    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties> Components { get; }
}
