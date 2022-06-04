using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class ModalSubmitInteraction : Interaction
{
    public override ModalSubmitInteractionData Data { get; }

    internal ModalSubmitInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data);
    }
}