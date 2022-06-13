namespace NetCord;

public class MessageActivity : IJsonModel<JsonModels.JsonMessageActivity>
{
    JsonModels.JsonMessageActivity IJsonModel<JsonModels.JsonMessageActivity>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonMessageActivity _jsonModel;

    public MessageActivityType Type => _jsonModel.Type;
    public string? PartyId => _jsonModel.PartyId;

    public MessageActivity(JsonModels.JsonMessageActivity jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
