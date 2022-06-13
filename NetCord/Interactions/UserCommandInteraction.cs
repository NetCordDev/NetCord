using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class UserCommandInteraction : ApplicationCommandInteraction
{
    public UserCommandInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client.Rest);
    }

    public override UserCommandInteractionData Data { get; }
}
