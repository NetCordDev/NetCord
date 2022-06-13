using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class ModalSubmitInteraction : Interaction
{
    public override ModalSubmitInteractionData Data { get; }

    public ModalSubmitInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data);
    }
}