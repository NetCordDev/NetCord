using NetCord.JsonModels;

namespace NetCord;

public class MenuInteraction : Interaction
{
    public override MenuInteractionData Data { get; }

    internal MenuInteraction(JsonInteraction jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data);
    }
}