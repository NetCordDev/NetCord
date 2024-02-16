using NetCord.Rest;

namespace NetCord;

public class MessageSticker(JsonModels.JsonMessageSticker jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonMessageSticker>
{
    JsonModels.JsonMessageSticker IJsonModel<JsonModels.JsonMessageSticker>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    public string Name => jsonModel.Name;

    public StickerFormat Format => jsonModel.Format;
}
