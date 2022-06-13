using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public abstract class ApplicationCommandInteraction : Interaction
{
    public abstract override ApplicationCommandInteractionData Data { get; }

    public ApplicationCommandInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
    }
}