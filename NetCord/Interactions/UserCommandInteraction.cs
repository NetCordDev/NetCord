using NetCord.JsonModels;

namespace NetCord;

public class UserCommandInteraction : ApplicationCommandInteraction
{
    internal UserCommandInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data, jsonEntity.GuildId, client.Rest);
    }

    public override UserCommandInteractionData Data { get; }
}
