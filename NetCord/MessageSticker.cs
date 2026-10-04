using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a sticker sent within a message object.
/// </summary>
public class MessageSticker(JsonModels.JsonMessageSticker jsonModel, RestClient client) : ClientEntity(jsonModel, client)
{
    /// <inheritdoc cref="Sticker.Name"/>
    public string Name { get; } = jsonModel.Name;

    /// <inheritdoc cref="Sticker.Format"/>
    public StickerFormat Format { get; } = jsonModel.Format;
}
