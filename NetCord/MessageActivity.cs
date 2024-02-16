namespace NetCord;

public class MessageActivity(JsonModels.JsonMessageActivity jsonModel) : IJsonModel<JsonModels.JsonMessageActivity>
{
    JsonModels.JsonMessageActivity IJsonModel<JsonModels.JsonMessageActivity>.JsonModel => jsonModel;

    public MessageActivityType Type => jsonModel.Type;
    public string? PartyId => jsonModel.PartyId;
}
