using NetCord.JsonModels;

namespace NetCord;

public class ApplicationCommandInteraction : Interaction
{
    public override ApplicationCommandInteractionData Data { get; }

    //public IReadOnlyCollection<ApplicationCommandParameter> Parameters => _jsonEntity.

    internal ApplicationCommandInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data, jsonEntity.GuildId, client.Rest);
    }
}