using NetCord.Rest;

namespace NetCord;

public class MessageInteraction : Entity
{
    private readonly JsonModels.JsonMessageInteraction _jsonModel;

    public override Snowflake Id => _jsonModel.Id;
    public InteractionType Type => _jsonModel.Type;
    public string Name => _jsonModel.Name;
    public User User { get; }

    public MessageInteraction(JsonModels.JsonMessageInteraction jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        User = new(jsonModel.User, client);
    }
}