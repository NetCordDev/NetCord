using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class ButtonInteraction : Interaction
{
    public override ButtonInteractionData Data { get; }

    public Message Message { get; }

    public ButtonInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data);
        if (jsonModel.GuildId.HasValue)
            Message = new(jsonModel.Message with { GuildId = jsonModel.GuildId }, client);
        else
            Message = new(jsonModel.Message, client);
    }
}