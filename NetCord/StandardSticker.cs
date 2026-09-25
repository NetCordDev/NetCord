using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an official Discord sticker.
/// </summary>
public class StandardSticker(JsonModels.JsonSticker jsonModel) : Sticker(jsonModel)
{
    /// <summary>
    /// The ID of the sticker's parent <see cref="StickerPack"/>.
    /// </summary>
    public ulong PackId => _jsonModel.PackId.GetValueOrDefault();

    /// <summary>
    /// The sticker's sort value within its parent <see cref="StickerPack"/>.
    /// </summary>
    public int? SortValue => _jsonModel.SortValue;
}
