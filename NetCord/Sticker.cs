namespace NetCord;

/// <summary>
/// Represents a sticker within Discord.
/// </summary>
public abstract class Sticker(JsonModels.JsonSticker jsonModel) : Entity, IJsonModel<JsonModels.JsonSticker>
{
    /// <inheritdoc />
    JsonModels.JsonSticker IJsonModel<JsonModels.JsonSticker>.JsonModel => jsonModel;

    /// <summary>
    /// The sticker's unique ID.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The sticker's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The sticker's description.
    /// </summary>
    public string Description => jsonModel.Description;

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
