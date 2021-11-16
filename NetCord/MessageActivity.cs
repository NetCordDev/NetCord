namespace NetCord;

public class MessageActivity
{
    private readonly JsonModels.JsonMessageActivity _jsonEntity;

    public MessageActivityType Type => _jsonEntity.Type;
    public string? PartyId => _jsonEntity.PartyId;

    internal MessageActivity(JsonModels.JsonMessageActivity jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
