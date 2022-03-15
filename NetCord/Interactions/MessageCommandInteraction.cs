using NetCord.JsonModels;

namespace NetCord;

public class MessageCommandInteraction : ApplicationCommandInteraction
{
    internal MessageCommandInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data, jsonEntity.GuildId, client.Rest);
    }

    public override MessageCommandInteractionData Data { get; }
}