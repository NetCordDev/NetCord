using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[CollectionBuilder(typeof(MediaGalleryProperties), nameof(Create))]
public partial class MediaGalleryProperties(IEnumerable<MediaGalleryItemProperties> items) : IComponentProperties, IMediaGalleryProperties, IEnumerable<MediaGalleryItemProperties>
{
    public MediaGalleryProperties() : this([])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.MediaGallery;

    public IEnumerable<MediaGalleryItemProperties> Items { get; set; } = items;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(MediaGalleryItemProperties item) => AddItems(item);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<MediaGalleryItemProperties> IEnumerable<MediaGalleryItemProperties>.GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Items).GetEnumerator();

    public void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.IMediaGalleryProperties);
    }
}

internal interface IMediaGalleryProperties : IComponentProperties
{
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
