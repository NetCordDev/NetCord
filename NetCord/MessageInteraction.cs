using NetCord.Rest;

namespace NetCord;

public class MessageInteraction(JsonModels.JsonMessageInteraction jsonModel, RestClient client) : Entity
{
    public override ulong Id => jsonModel.Id;
    public InteractionType Type => jsonModel.Type;
    public string Name => jsonModel.Name;
    public User User { get; } = new(jsonModel.User, client);
}
