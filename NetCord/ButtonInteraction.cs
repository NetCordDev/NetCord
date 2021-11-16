using NetCord.JsonModels;

namespace NetCord;

public class ButtonInteraction : Interaction
{
    public override ButtonInteractionData Data { get; }

    internal ButtonInteraction(JsonInteraction jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data);
    }
}