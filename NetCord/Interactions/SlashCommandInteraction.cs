using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class SlashCommandInteraction : ApplicationCommandInteraction
{
    internal SlashCommandInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data, jsonEntity.GuildId, client.Rest);
    }

    public override SlashCommandInteractionData Data { get; }
}