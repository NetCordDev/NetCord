using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a sticker sent within a message object.
/// </summary>
public class MessageSticker(JsonModels.JsonMessageSticker jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonMessageSticker>
{
    JsonModels.JsonMessageSticker IJsonModel<JsonModels.JsonMessageSticker>.JsonModel => jsonModel;

    /// <inheritdoc cref="Sticker.Id"/>
    public override ulong Id => jsonModel.Id;

    /// <inheritdoc cref="Sticker.Name"/>
    public string Name => jsonModel.Name;

    /// <inheritdoc cref="Sticker.Format"/>
    public StickerFormat Format => jsonModel.Format;
}
