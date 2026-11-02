using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a sticker within Discord.
/// </summary>
public abstract class Sticker(JsonModels.JsonSticker jsonModel) : Entity(jsonModel)
{
    /// <summary>
    /// The sticker's name.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// The sticker's description.
    /// </summary>
    public string Description { get; } = jsonModel.Description;

    /// <summary>
    /// A list of sticker tags, used for autocomplete/suggestions.
    /// </summary>
    /// <remarks>
    /// The total character count for all entries cannot exceed 200.
    /// </remarks>
    public IReadOnlyList<string> Tags { get; } = jsonModel.Tags.Split(',');

    /// <summary>
    /// The sticker's image format.
    /// </summary>
    public StickerFormat Format => jsonModel.Format;

    /// <inheritdoc cref="ImageUrl.Sticker" />
    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.Sticker(Id, Format, format);
}

/// <summary>
/// Represents an official Discord sticker.
/// </summary>
public class StandardSticker(JsonModels.JsonSticker jsonModel) : Sticker(jsonModel)
{
    /// <summary>
    /// The ID of the sticker's parent <see cref="StickerPack"/>.
    /// </summary>
    public ulong PackId { get; } = jsonModel.PackId.GetValueOrDefault();

    /// <summary>
    /// The sticker's sort value within its parent <see cref="StickerPack"/>.
    /// </summary>
    public int? SortValue { get; } = jsonModel.SortValue;
}

/// <summary>
/// Represents a custom guild sticker.
/// </summary>
public partial class GuildSticker(JsonModels.JsonSticker jsonModel, RestClient client) : Sticker(jsonModel)
{
    /// <summary>
    /// Whether the sticker is available for use. Can be <see langword="false"/> if server boosts are lost.
    /// </summary>
    public bool? Available { get; } = jsonModel.Available;

    /// <summary>
    /// The ID corresponding to the sticker's parent guild.
    /// </summary>
    public ulong GuildId { get; } = jsonModel.GuildId.GetValueOrDefault();

    /// <summary>
    /// The user that uploaded the sticker.
    /// </summary>
    public User? User { get; } = jsonModel.User is { } user ? new(user, client) : null;
}
