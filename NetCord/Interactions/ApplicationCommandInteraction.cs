using NetCord.JsonModels;

namespace NetCord;

public abstract class ApplicationCommandInteraction : Interaction
{
    public abstract override ApplicationCommandInteractionData Data { get; }
    //public IReadOnlyCollection<ApplicationCommandParameter> Parameters => _jsonEntity.

    internal ApplicationCommandInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
    }
}