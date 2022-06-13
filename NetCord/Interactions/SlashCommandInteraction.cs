using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    public SlashCommandInteraction(JsonInteraction jsonModel, GatewayClient client) : base(jsonModel, client)
    {
        Data = new(jsonModel.Data, jsonModel.GuildId, client.Rest);
    }

    public override SlashCommandInteractionData Data { get; }
}