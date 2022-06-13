using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    public MessageCommandInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client.Rest);
    }

    public override MessageCommandInteractionData Data { get; }
}