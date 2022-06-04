using NetCord.Gateway;
using NetCord.JsonModels;

namespace NetCord;

public class ApplicationCommandAutocompleteInteraction : Interaction
{
    public override ApplicationCommandAutocompleteInteractionData Data { get; }

    internal ApplicationCommandAutocompleteInteraction(JsonInteraction jsonEntity, GatewayClient client) : base(jsonEntity, client)
    {
        Data = new(jsonEntity.Data, jsonEntity.GuildId, client.Rest);
    }
}