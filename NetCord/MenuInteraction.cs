using NetCord.JsonModels;

namespace NetCord;

public class MenuInteraction : ButtonInteraction
{
    public override MenuInteractionData Data { get; }

    internal MenuInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data);
    }
}