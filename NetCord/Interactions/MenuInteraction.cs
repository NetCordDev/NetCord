using NetCord.JsonModels;

namespace NetCord;

public class MenuInteraction : Interaction
{
    public override MenuInteractionData Data { get; }
    public Message Message { get; }

    internal MenuInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data);
        if (jsonEntity.GuildId.HasValue)
            Message = new(jsonEntity.Message with { GuildId = jsonEntity.GuildId }, client);
        else
            Message = new(jsonEntity.Message, client);
    }
}