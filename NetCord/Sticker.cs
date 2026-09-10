namespace NetCord;

/// <summary>
/// Represents a sticker within Discord.
/// </summary>
public abstract class Sticker : Entity, IJsonModel<JsonModels.JsonSticker>
{
    JsonModels.JsonSticker IJsonModel<JsonModels.JsonSticker>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonSticker _jsonModel;

    /// <summary>
    /// The sticker's unique ID.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The sticker's name.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// The sticker's description.
    /// </summary>
    public string Description => _jsonModel.Description;

    /// <summary>
    /// A list of sticker tags, used for autocomplete/suggestions.
    /// </summary>
    /// <remarks>
    /// The total character count for all entries cannot exceed 200.
    /// </remarks>
    public IReadOnlyList<string> Tags { get; }

    /// <summary>
    /// The sticker's image format.
    /// </summary>
    public StickerFormat Format => _jsonModel.Format;

    private protected Sticker(JsonModels.JsonSticker jsonModel)
    {
        _jsonModel = jsonModel;
        Tags = _jsonModel.Tags.Split(',');
    }

    /// <inheritdoc cref="ImageUrl.Sticker" />
    public ImageUrl GetImageUrl(ImageFormat format) => ImageUrl.Sticker(Id, Format, format);
}
